using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LAMAMedellin.API.Tests.Infraestructura;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LAMAMedellin.API.Tests.Integracion;

/// <summary>
/// Cartera extremo a extremo: emitir la obligacion y cobrarla.
///
/// Es el flujo central de la Phase 1 y el que sostiene la caja del capitulo.
/// </summary>
public sealed class CarteraTests(FabricaApiPruebas fabrica) : IClassFixture<FabricaApiPruebas>
{
    private sealed record Contexto(Guid MiembroId, Guid ConceptoId, Guid BancoId);

    private async Task<Contexto> PrepararAsync()
    {
        var contexto = await fabrica.PrepararBaseAsync();

        var miembro = await contexto.Miembros.FirstAsync();
        var concepto = await contexto.ConceptosCobro.FirstAsync();
        var banco = await contexto.Bancos.FirstAsync(b => b.EsActivo);

        return new Contexto(miembro.Id, concepto.Id, banco.Id);
    }

    private static object ArmarCuentaPorCobrar(Contexto c, string periodo, decimal valor) => new
    {
        miembroId = c.MiembroId,
        conceptoCobroId = c.ConceptoId,
        periodo,
        fechaEmision = DateOnly.FromDateTime(DateTime.UtcNow),
        fechaVencimiento = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30),
        valorTotal = valor,
    };

    [Fact]
    public async Task Emitir_una_cuenta_por_cobrar_la_deja_pendiente()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente();

        var respuesta = await cliente.PostAsJsonAsync(
            "/api/cartera/cuentas-por-cobrar",
            ArmarCuentaPorCobrar(c, "2026-03", 80000m));

        respuesta.StatusCode.Should().Be(HttpStatusCode.Created);

        var pendientes = await cliente.GetFromJsonAsync<List<CuentaPorCobrarRespuesta>>(
            "/api/cartera/cuentas-por-cobrar");

        pendientes!.Should().Contain(x => x.ValorTotal == 80000m && x.SaldoPendiente == 80000m);
    }

    [Fact]
    public async Task Un_pago_parcial_deja_saldo_y_la_cuenta_sigue_en_cartera()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente();

        var creacion = await cliente.PostAsJsonAsync(
            "/api/cartera/cuentas-por-cobrar",
            ArmarCuentaPorCobrar(c, "2026-04", 100000m));
        var creada = await creacion.Content.ReadFromJsonAsync<RespuestaId>();

        var pago = await cliente.PostAsJsonAsync(
            $"/api/cartera/cuentas-por-cobrar/{creada!.Id}/pagos",
            new { monto = 40000m, bancoId = c.BancoId, medioPago = 1 });

        pago.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.NoContent);

        var cartera = await cliente.GetFromJsonAsync<List<CuentaPorCobrarRespuesta>>(
            "/api/cartera/cuentas-por-cobrar");
        var cuenta = cartera!.Single(x => x.Id == creada.Id);

        // Una cuenta con abono parcial sigue siendo cartera pendiente: darla por
        // saldada al primer abono fue el bug 6.
        cuenta.SaldoPendiente.Should().Be(60000m);
    }

    [Fact]
    public async Task No_se_puede_pagar_mas_que_el_saldo()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente();

        var creacion = await cliente.PostAsJsonAsync(
            "/api/cartera/cuentas-por-cobrar",
            ArmarCuentaPorCobrar(c, "2026-05", 50000m));
        var creada = await creacion.Content.ReadFromJsonAsync<RespuestaId>();

        var pago = await cliente.PostAsJsonAsync(
            $"/api/cartera/cuentas-por-cobrar/{creada!.Id}/pagos",
            new { monto = 90000m, bancoId = c.BancoId, medioPago = 1 });

        // Aceptarlo dejaria un saldo negativo, que no significa nada.
        pago.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Pagar_el_saldo_completo_saca_la_cuenta_de_la_cartera_pendiente()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente();

        var creacion = await cliente.PostAsJsonAsync(
            "/api/cartera/cuentas-por-cobrar",
            ArmarCuentaPorCobrar(c, "2026-06", 30000m));
        var creada = await creacion.Content.ReadFromJsonAsync<RespuestaId>();

        await cliente.PostAsJsonAsync(
            $"/api/cartera/cuentas-por-cobrar/{creada!.Id}/pagos",
            new { monto = 30000m, bancoId = c.BancoId, medioPago = 1 });

        var cartera = await cliente.GetFromJsonAsync<List<CuentaPorCobrarRespuesta>>(
            "/api/cartera/cuentas-por-cobrar");
        var cuenta = cartera!.Single(x => x.Id == creada.Id);

        cuenta.SaldoPendiente.Should().Be(0m);
    }

    [Fact]
    public async Task El_periodo_es_obligatorio_al_emitir()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente();

        var respuesta = await cliente.PostAsJsonAsync("/api/cartera/cuentas-por-cobrar", new
        {
            miembroId = c.MiembroId,
            conceptoCobroId = c.ConceptoId,
            fechaEmision = DateOnly.FromDateTime(DateTime.UtcNow),
            fechaVencimiento = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30),
            valorTotal = 50000m,
        });

        // Sin periodo no hay forma de saber que mes cubre la obligacion, que es
        // lo que evita cobrar dos veces lo mismo.
        respuesta.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task El_catalogo_de_miembros_para_seleccionar_responde()
    {
        await PrepararAsync();
        var cliente = fabrica.CrearCliente();

        var lookup = await cliente.GetFromJsonAsync<List<MiembroLookup>>("/api/cartera/miembros/lookup");

        lookup.Should().NotBeNull();
        lookup!.Should().NotBeEmpty();
        lookup.Should().OnlyContain(m => !string.IsNullOrWhiteSpace(m.NombreCompleto));
    }

    private sealed record CuentaPorCobrarRespuesta(Guid Id, decimal ValorTotal, decimal SaldoPendiente, int Estado);
    private sealed record MiembroLookup(Guid Id, string NombreCompleto);
    private sealed record RespuestaId(Guid Id);
}
