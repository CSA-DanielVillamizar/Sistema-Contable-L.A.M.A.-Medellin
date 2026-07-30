using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LAMAMedellin.API.Tests.Infraestructura;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LAMAMedellin.API.Tests.Integracion;

/// <summary>
/// Administracion de cuentas bancarias, extremo a extremo por HTTP.
///
/// Es el catalogo del que cuelga toda la tesoreria y hasta ahora solo se podia
/// sembrar: no habia forma de dar de alta una cuenta ni de corregir la que
/// quedo con un numero provisional tras la consolidacion.
/// </summary>
public sealed class CuentasBancariasTests(FabricaApiPruebas fabrica) : IClassFixture<FabricaApiPruebas>
{
    private const string Ruta = "/api/tesoreria/cuentas-bancarias";

    private async Task<Guid> ObtenerCuentaDisponibleAsync()
    {
        var contexto = await fabrica.PrepararBaseAsync();

        var cuenta = await contexto.CuentasContables
            .FirstAsync(c => c.Codigo.StartsWith("11") && c.PermiteMovimiento);

        return cuenta.Id;
    }

    [Fact]
    public async Task Crear_una_cuenta_bancaria_la_deja_disponible_en_el_listado()
    {
        var cuentaContableId = await ObtenerCuentaDisponibleAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var creacion = await cliente.PostAsJsonAsync(Ruta, new
        {
            nombre = "Bancolombia - Ahorros",
            numeroCuenta = $"CTA-{Guid.NewGuid():N}"[..20],
            cuentaContableId,
        });

        creacion.StatusCode.Should().Be(HttpStatusCode.Created);

        var listado = await cliente.GetFromJsonAsync<List<CuentaBancariaRespuesta>>(Ruta);

        listado.Should().NotBeNull();
        listado!.Should().Contain(c => c.Nombre == "Bancolombia - Ahorros");
    }

    [Fact]
    public async Task Una_cuenta_nace_con_saldo_en_cero()
    {
        var cuentaContableId = await ObtenerCuentaDisponibleAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        await cliente.PostAsJsonAsync(Ruta, new
        {
            nombre = "Cuenta recien creada",
            numeroCuenta = $"NUEVA-{Guid.NewGuid():N}"[..20],
            cuentaContableId,
        });

        var listado = await cliente.GetFromJsonAsync<List<CuentaBancariaRespuesta>>(Ruta);
        var creada = listado!.Single(c => c.Nombre == "Cuenta recien creada");

        // El saldo se deriva de los movimientos. Si se pudiera sembrar desde el
        // alta quedaria desligado del libro desde el primer dia.
        creada.SaldoActual.Should().Be(0m);
    }

    [Fact]
    public async Task No_se_admite_una_cuenta_contable_fuera_del_disponible()
    {
        var contexto = await fabrica.PrepararBaseAsync();
        var cuentaIngresos = await contexto.CuentasContables
            .FirstAsync(c => c.Codigo.StartsWith("4") && c.PermiteMovimiento);

        var cliente = fabrica.CrearCliente("Tesorero");

        var respuesta = await cliente.PostAsJsonAsync(Ruta, new
        {
            nombre = "Cuenta mal clasificada",
            numeroCuenta = $"MAL-{Guid.NewGuid():N}"[..20],
            cuentaContableId = cuentaIngresos.Id,
        });

        // Respaldarla en una cuenta de ingresos dejaria la contrapartida en un
        // rubro que no representa efectivo.
        respuesta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task No_se_admiten_dos_cuentas_con_el_mismo_numero()
    {
        var cuentaContableId = await ObtenerCuentaDisponibleAsync();
        var cliente = fabrica.CrearCliente("Tesorero");
        var numero = $"REPE-{Guid.NewGuid():N}"[..20];

        var primera = await cliente.PostAsJsonAsync(Ruta, new
        {
            nombre = "Primera",
            numeroCuenta = numero,
            cuentaContableId,
        });
        primera.StatusCode.Should().Be(HttpStatusCode.Created);

        var segunda = await cliente.PostAsJsonAsync(Ruta, new
        {
            nombre = "Segunda con el mismo numero",
            numeroCuenta = numero,
            cuentaContableId,
        });

        // Dos registros con el mismo numero competirian por los mismos
        // movimientos del extracto.
        segunda.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Editar_una_cuenta_cambia_su_nombre_y_su_numero()
    {
        var cuentaContableId = await ObtenerCuentaDisponibleAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var creacion = await cliente.PostAsJsonAsync(Ruta, new
        {
            nombre = "Nombre provisional",
            numeroCuenta = $"PROV-{Guid.NewGuid():N}"[..20],
            cuentaContableId,
        });
        var creada = await creacion.Content.ReadFromJsonAsync<RespuestaId>();

        var edicion = await cliente.PutAsJsonAsync($"{Ruta}/{creada!.Id}", new
        {
            nombre = "Bancolombia - Corriente 123",
            numeroCuenta = "123-456789-01",
            cuentaContableId,
        });

        edicion.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var listado = await cliente.GetFromJsonAsync<List<CuentaBancariaRespuesta>>(Ruta);
        var actualizada = listado!.Single(c => c.Id == creada.Id);

        actualizada.Nombre.Should().Be("Bancolombia - Corriente 123");
        actualizada.NumeroCuenta.Should().Be("123-456789-01");
    }

    [Fact]
    public async Task Una_cuenta_desactivada_desaparece_del_listado_por_defecto()
    {
        var cuentaContableId = await ObtenerCuentaDisponibleAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var creacion = await cliente.PostAsJsonAsync(Ruta, new
        {
            nombre = "Cuenta a dar de baja",
            numeroCuenta = $"BAJA-{Guid.NewGuid():N}"[..20],
            cuentaContableId,
        });
        var creada = await creacion.Content.ReadFromJsonAsync<RespuestaId>();

        var baja = await cliente.PatchAsJsonAsync($"{Ruta}/{creada!.Id}/estado", new { esActivo = false });
        baja.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var activas = await cliente.GetFromJsonAsync<List<CuentaBancariaRespuesta>>(Ruta);
        activas!.Should().NotContain(c => c.Id == creada.Id);

        // Pero conserva su historia: sigue estando y se puede reactivar.
        var todas = await cliente.GetFromJsonAsync<List<CuentaBancariaRespuesta>>($"{Ruta}?incluirInactivas=true");
        todas!.Should().Contain(c => c.Id == creada.Id);
    }

    [Fact]
    public async Task Un_rol_sin_permiso_no_puede_administrar_cuentas()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Contador");

        var respuesta = await cliente.GetAsync(Ruta);

        // La pantalla oculta la opcion, pero quien decide es el backend.
        respuesta.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private sealed record CuentaBancariaRespuesta(
        Guid Id,
        string Nombre,
        string NumeroCuenta,
        decimal SaldoActual,
        bool EsActivo,
        Guid CuentaContableId);

    private sealed record RespuestaId(Guid Id);
}
