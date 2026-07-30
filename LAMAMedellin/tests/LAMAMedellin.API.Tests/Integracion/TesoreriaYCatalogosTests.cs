using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LAMAMedellin.API.Tests.Infraestructura;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LAMAMedellin.API.Tests.Integracion;

/// <summary>
/// Tesoreria, catalogos y los modulos de club, extremo a extremo.
///
/// Cubre en particular los contratos que el binding descartaba en silencio:
/// campos que el formulario enviaba con otro nombre, u obligatorios que no
/// viajaban. Nada de eso fallaba de forma visible; el registro simplemente
/// quedaba incompleto.
/// </summary>
public sealed class TesoreriaYCatalogosTests(FabricaApiPruebas fabrica) : IClassFixture<FabricaApiPruebas>
{
    [Fact]
    public async Task El_catalogo_de_bancos_expone_el_nombre_de_la_cuenta()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente();

        var bancos = await cliente.GetFromJsonAsync<List<CatalogoBanco>>("/api/transacciones/bancos");

        bancos.Should().NotBeNull();
        bancos!.Should().NotBeEmpty();

        // Sin nombre, el desplegable mostraba el numero de cuenta crudo y el
        // usuario no tenia como reconocer cual elegir.
        bancos.Should().OnlyContain(b => !string.IsNullOrWhiteSpace(b.Nombre));
    }

    [Fact]
    public async Task El_catalogo_de_bancos_no_ofrece_cuentas_inactivas()
    {
        var contexto = await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente();

        var banco = await contexto.Bancos.FirstAsync(b => b.EsActivo);
        await cliente.PatchAsJsonAsync(
            $"/api/tesoreria/cuentas-bancarias/{banco.Id}/estado", new { esActivo = false });

        var bancos = await cliente.GetFromJsonAsync<List<CatalogoBanco>>("/api/transacciones/bancos");

        // Ofrecerla solo consigue que el registro falle al guardar: el dominio
        // rechaza movimientos contra una cuenta dada de baja.
        bancos!.Should().NotContain(b => b.Id == banco.Id);
    }

    [Fact]
    public async Task Crear_un_producto_conserva_el_stock_inicial_y_la_cantidad_minima()
    {
        var contexto = await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Admin");

        var cuentaIngreso = await contexto.CuentasContables
            .FirstAsync(c => c.Codigo.StartsWith("4") && c.PermiteMovimiento);

        var respuesta = await cliente.PostAsJsonAsync("/api/merchandising/productos", new
        {
            nombre = "Parche bordado",
            codigoSKU = $"SKU-{Guid.NewGuid():N}"[..12],
            precioVenta = 35000m,
            cantidadEnStock = 25,
            cantidadMinima = 5,
            cuentaContableIngresoId = cuentaIngreso.Id,
        });

        respuesta.StatusCode.Should().Be(HttpStatusCode.Created);

        var creado = await respuesta.Content.ReadFromJsonAsync<RespuestaId>();
        var producto = await contexto.Productos.AsNoTracking().FirstAsync(p => p.Id == creado!.Id);

        // El formulario capturaba ambos, pero el comando no los declaraba y los
        // productos nacian en cero sin que nadie se enterara.
        producto.CantidadEnStock.Should().Be(25);
        producto.CantidadMinima.Should().Be(5);
    }

    [Fact]
    public async Task Crear_un_centro_de_costo_lo_deja_disponible_para_los_asientos()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Admin");

        var creacion = await cliente.PostAsJsonAsync("/api/configuracion/centros-costo", new
        {
            nombre = "Rodada Semana Santa",
            tipo = 4,
        });

        creacion.StatusCode.Should().Be(HttpStatusCode.Created);

        var catalogo = await cliente.GetFromJsonAsync<List<CatalogoCentroCosto>>("/api/transacciones/centros-costo");
        catalogo!.Should().Contain(c => c.Nombre == "Rodada Semana Santa");
    }

    [Fact]
    public async Task Un_evento_programado_se_puede_editar()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente();

        var creacion = await cliente.PostAsJsonAsync("/api/eventos", new
        {
            nombre = "Rodada inicial",
            descripcion = "Salida mensual",
            fechaProgramada = DateTime.UtcNow.AddDays(20),
            lugarEncuentro = "Parque de El Poblado",
            tipoEvento = 1,
            destino = "Santa Fe de Antioquia",
        });
        creacion.StatusCode.Should().Be(HttpStatusCode.Created);

        var creado = await creacion.Content.ReadFromJsonAsync<RespuestaId>();

        var edicion = await cliente.PutAsJsonAsync($"/api/eventos/{creado!.Id}", new
        {
            nombre = "Rodada reprogramada",
            descripcion = "Salida mensual",
            fechaProgramada = DateTime.UtcNow.AddDays(27),
            lugarEncuentro = "Parque de El Poblado",
            tipoEvento = 1,
            destino = "Guatape",
        });

        // El boton de editar existia en pantalla pero no habia endpoint: la
        // accion respondia 404.
        edicion.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.OK);
    }

    /// <summary>
    /// Queda fuera del conjunto automatico a proposito: el tablero suma
    /// columnas decimal en la base y SQLite no implementa ese agregado. SQL
    /// Server si, de modo que no es un fallo de la aplicacion sino un limite
    /// del motor con el que corren estas pruebas. Cubrirlo exige levantarlas
    /// contra SQL Server.
    /// </summary>
    [Fact(Skip = "Requiere SQL Server: SQLite no puede aplicar SUM sobre decimal.")]
    public async Task El_tablero_responde_con_los_saldos()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente();

        var respuesta = await cliente.GetAsync("/api/dashboard/resumen");

        respuesta.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task El_plan_de_cuentas_marca_cuales_exigen_tercero()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente();

        var cuentas = await cliente.GetFromJsonAsync<List<CuentaContableRespuesta>>("/api/cuentas-contables");

        // El modal necesita este dato para avisar y para validar antes de
        // enviar; si el DTO dejara de exponerlo, el 422 volveria sin aviso.
        cuentas!.Should().Contain(c => c.PermiteMovimiento && c.ExigeTercero);
    }

    private sealed record CatalogoBanco(Guid Id, string Nombre, string NumeroCuenta);
    private sealed record CatalogoCentroCosto(Guid Id, string Nombre);
    private sealed record CuentaContableRespuesta(Guid Id, string Codigo, bool PermiteMovimiento, bool ExigeTercero);
    private sealed record RespuestaId(Guid Id);
}
