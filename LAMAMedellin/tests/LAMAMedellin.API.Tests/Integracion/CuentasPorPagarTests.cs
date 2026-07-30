using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LAMAMedellin.API.Tests.Infraestructura;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LAMAMedellin.API.Tests.Integracion;

/// <summary>
/// Cuentas por pagar (historias 1-13 y 1-14), extremo a extremo.
///
/// Antes de esto una factura pendiente no existia en ninguna parte hasta que se
/// pagaba, de modo que el pasivo del capitulo quedaba fuera del balance.
/// </summary>
public sealed class CuentasPorPagarTests(FabricaApiPruebas fabrica) : IClassFixture<FabricaApiPruebas>
{
    private const string Ruta = "/api/cuentas-por-pagar";

    private sealed record Contexto(Guid CuentaGasto, Guid CuentaIngreso, Guid CentroCosto, Guid Banco);

    private async Task<Contexto> PrepararAsync()
    {
        var contexto = await fabrica.PrepararBaseAsync();

        var gasto = await contexto.CuentasContables
            .FirstAsync(c => c.Codigo.StartsWith("5") && c.PermiteMovimiento);
        var ingreso = await contexto.CuentasContables
            .FirstAsync(c => c.Codigo.StartsWith("4") && c.PermiteMovimiento);
        var centro = await contexto.CentrosCosto.FirstAsync();
        var banco = await contexto.Bancos.FirstAsync(b => b.EsActivo);

        return new Contexto(gasto.Id, ingreso.Id, centro.Id, banco.Id);
    }

    /// <summary>
    /// El banco sembrado nace en cero y el dominio rechaza un egreso sin saldo,
    /// con razon. Para probar el pago hay que meter dinero antes, igual que en
    /// la operacion real.
    /// </summary>
    private async Task FondearBancoAsync(HttpClient cliente, Contexto c, decimal monto)
    {
        var respuesta = await cliente.PostAsJsonAsync("/api/tesoreria/ingresos", new
        {
            monto,
            concepto = "Fondeo para pruebas de pago",
            terceroId = (Guid?)null,
            cuentaContableId = c.CuentaIngreso,
            bancoId = c.Banco,
            centroCostoId = c.CentroCosto,
            medioPago = 1,
        });

        respuesta.StatusCode.Should().Be(HttpStatusCode.Created, "sin fondeo el egreso no puede probarse");
    }

    private static object Factura(Contexto c, string numero, decimal valor) => new
    {
        nombreProveedor = "Taller Mecanico El Piston",
        nitProveedor = "900123456-1",
        numeroFactura = numero,
        concepto = "Mantenimiento de motos del club",
        cuentaContableGastoId = c.CuentaGasto,
        centroCostoId = c.CentroCosto,
        fechaEmision = DateOnly.FromDateTime(DateTime.UtcNow),
        fechaVencimiento = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30),
        valorTotal = valor,
    };

    [Fact]
    public async Task Registrar_una_factura_la_deja_pendiente_por_su_valor_total()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var respuesta = await cliente.PostAsJsonAsync(Ruta, Factura(c, $"F-{Guid.NewGuid():N}"[..10], 500000m));
        respuesta.StatusCode.Should().Be(HttpStatusCode.Created);

        var listado = await cliente.GetFromJsonAsync<List<CuentaPorPagarRespuesta>>(Ruta);
        listado!.Should().Contain(x => x.ValorTotal == 500000m && x.SaldoPendiente == 500000m && x.Estado == 1);
    }

    [Fact]
    public async Task No_se_admite_la_misma_factura_del_mismo_proveedor_dos_veces()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente("Tesorero");
        var numero = $"DUP-{Guid.NewGuid():N}"[..10];

        var primera = await cliente.PostAsJsonAsync(Ruta, Factura(c, numero, 100000m));
        primera.StatusCode.Should().Be(HttpStatusCode.Created);

        var segunda = await cliente.PostAsJsonAsync(Ruta, Factura(c, numero, 100000m));

        // Registrarla dos veces duplicaria el pasivo y el gasto.
        segunda.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Una_factura_no_puede_imputarse_a_una_cuenta_de_ingresos()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var respuesta = await cliente.PostAsJsonAsync(Ruta, new
        {
            nombreProveedor = "Proveedor mal imputado",
            nitProveedor = "900999999-9",
            numeroFactura = $"MAL-{Guid.NewGuid():N}"[..10],
            concepto = "Imputacion incorrecta",
            cuentaContableGastoId = c.CuentaIngreso,
            centroCostoId = c.CentroCosto,
            fechaEmision = DateOnly.FromDateTime(DateTime.UtcNow),
            fechaVencimiento = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30),
            valorTotal = 50000m,
        });

        respuesta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var problema = await respuesta.Content.ReadFromJsonAsync<DetalleProblema>();
        problema!.Detail.Should().Contain("gasto");
    }

    [Fact]
    public async Task Un_pago_parcial_deja_saldo_y_la_factura_sigue_pendiente()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        await FondearBancoAsync(cliente, c, 1000000m);

        var creacion = await cliente.PostAsJsonAsync(Ruta, Factura(c, $"PAR-{Guid.NewGuid():N}"[..10], 300000m));
        var creada = await creacion.Content.ReadFromJsonAsync<RespuestaId>();

        var pago = await cliente.PostAsJsonAsync($"{Ruta}/{creada!.Id}/pagos", new
        {
            monto = 100000m,
            bancoId = c.Banco,
            medioPago = 1,
        });
        pago.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var listado = await cliente.GetFromJsonAsync<List<CuentaPorPagarRespuesta>>(Ruta);
        var cuenta = listado!.Single(x => x.Id == creada.Id);

        cuenta.SaldoPendiente.Should().Be(200000m);
        cuenta.Estado.Should().Be(2, "un abono parcial no salda la obligacion");
    }

    [Fact]
    public async Task Pagar_el_saldo_completo_marca_la_factura_como_pagada()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        await FondearBancoAsync(cliente, c, 1000000m);

        var creacion = await cliente.PostAsJsonAsync(Ruta, Factura(c, $"TOT-{Guid.NewGuid():N}"[..10], 80000m));
        var creada = await creacion.Content.ReadFromJsonAsync<RespuestaId>();

        await cliente.PostAsJsonAsync($"{Ruta}/{creada!.Id}/pagos", new
        {
            monto = 80000m,
            bancoId = c.Banco,
            medioPago = 1,
        });

        var listado = await cliente.GetFromJsonAsync<List<CuentaPorPagarRespuesta>>(Ruta);
        var cuenta = listado!.Single(x => x.Id == creada.Id);

        cuenta.SaldoPendiente.Should().Be(0m);
        cuenta.Estado.Should().Be(3);
    }

    [Fact]
    public async Task No_se_puede_pagar_mas_que_el_saldo()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var creacion = await cliente.PostAsJsonAsync(Ruta, Factura(c, $"EXC-{Guid.NewGuid():N}"[..10], 50000m));
        var creada = await creacion.Content.ReadFromJsonAsync<RespuestaId>();

        var pago = await cliente.PostAsJsonAsync($"{Ruta}/{creada!.Id}/pagos", new
        {
            monto = 90000m,
            bancoId = c.Banco,
            medioPago = 1,
        });

        pago.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Una_factura_con_pagos_ya_no_se_puede_anular()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        await FondearBancoAsync(cliente, c, 1000000m);

        var creacion = await cliente.PostAsJsonAsync(Ruta, Factura(c, $"ANU-{Guid.NewGuid():N}"[..10], 70000m));
        var creada = await creacion.Content.ReadFromJsonAsync<RespuestaId>();

        await cliente.PostAsJsonAsync($"{Ruta}/{creada!.Id}/pagos", new
        {
            monto = 20000m,
            bancoId = c.Banco,
            medioPago = 1,
        });

        var anulacion = await cliente.PostAsync($"{Ruta}/{creada.Id}/anular", null);

        // Ya movio dinero: anularla dejaria ese pago sin explicacion.
        anulacion.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Una_factura_sin_pagos_si_se_puede_anular()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var creacion = await cliente.PostAsJsonAsync(Ruta, Factura(c, $"OK-{Guid.NewGuid():N}"[..10], 40000m));
        var creada = await creacion.Content.ReadFromJsonAsync<RespuestaId>();

        var anulacion = await cliente.PostAsync($"{Ruta}/{creada!.Id}/anular", null);
        anulacion.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var vigentes = await cliente.GetFromJsonAsync<List<CuentaPorPagarRespuesta>>(Ruta);
        vigentes!.Should().NotContain(x => x.Id == creada.Id);
    }

    [Fact]
    public async Task Un_rol_fuera_de_la_matriz_no_registra_facturas()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente("Junta");

        var respuesta = await cliente.PostAsJsonAsync(Ruta, Factura(c, "SIN-PERMISO", 10000m));

        respuesta.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private sealed record CuentaPorPagarRespuesta(
        Guid Id,
        string NombreProveedor,
        string NumeroFactura,
        decimal ValorTotal,
        decimal SaldoPendiente,
        int Estado,
        bool EstaVencida);

    private sealed record DetalleProblema(string? Title, string? Detail);
    private sealed record RespuestaId(Guid Id);
}
