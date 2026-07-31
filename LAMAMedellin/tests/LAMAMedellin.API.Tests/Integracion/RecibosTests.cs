using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LAMAMedellin.API.Tests.Infraestructura;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LAMAMedellin.API.Tests.Integracion;

/// <summary>
/// Recibos en PDF con QR y verificacion publica (historia 1-7).
/// </summary>
public sealed class RecibosTests(FabricaApiPruebas fabrica) : IClassFixture<FabricaApiPruebas>
{
    private async Task<string> CrearComprobanteAsync(HttpClient cliente)
    {
        var contexto = await fabrica.PrepararBaseAsync();

        var cuenta = await contexto.CuentasContables.FirstAsync(c => c.PermiteMovimiento && !c.ExigeTercero);
        var centro = await contexto.CentrosCosto.FirstAsync();

        var respuesta = await cliente.PostAsJsonAsync("/api/comprobantes", new
        {
            fecha = DateTime.UtcNow.Date,
            tipo = (int)TipoComprobante.Diario,
            descripcion = "Movimiento con recibo",
            asientos = new object[]
            {
                new { cuentaContableId = cuenta.Id, terceroId = (Guid?)null, centroCostoId = centro.Id, debe = 25000m, haber = 0m, referencia = "Debito" },
                new { cuentaContableId = cuenta.Id, terceroId = (Guid?)null, centroCostoId = centro.Id, debe = 0m, haber = 25000m, referencia = "Credito" },
            },
        });

        respuesta.StatusCode.Should().Be(HttpStatusCode.Created);

        var creado = await respuesta.Content.ReadFromJsonAsync<RespuestaId>();
        var comprobante = await contexto.Comprobantes.AsNoTracking().FirstAsync(c => c.Id == creado!.Id);

        return comprobante.NumeroConsecutivo;
    }

    [Fact]
    public async Task El_recibo_se_descarga_como_PDF()
    {
        var cliente = fabrica.CrearCliente();
        var consecutivo = await CrearComprobanteAsync(cliente);

        var respuesta = await cliente.GetAsync($"/api/recibos/{consecutivo}/pdf");

        respuesta.StatusCode.Should().Be(HttpStatusCode.OK);
        respuesta.Content.Headers.ContentType!.MediaType.Should().Be("application/pdf");

        var bytes = await respuesta.Content.ReadAsByteArrayAsync();
        bytes.Should().NotBeEmpty();

        // Firma de un PDF valido: %PDF
        System.Text.Encoding.ASCII.GetString(bytes[..4]).Should().Be("%PDF");
    }

    [Fact]
    public async Task La_verificacion_publica_no_exige_sesion()
    {
        var cliente = fabrica.CrearCliente();
        var consecutivo = await CrearComprobanteAsync(cliente);

        // Sin cabecera de roles: quien recibe un recibo en papel no tiene
        // cuenta en el sistema.
        var anonimo = fabrica.CreateClient();
        var respuesta = await anonimo.GetAsync($"/api/recibos/verificar/{consecutivo}");

        respuesta.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task La_verificacion_publica_no_expone_datos_sensibles()
    {
        var cliente = fabrica.CrearCliente();
        var consecutivo = await CrearComprobanteAsync(cliente);

        var anonimo = fabrica.CreateClient();
        var cuerpo = await anonimo.GetStringAsync($"/api/recibos/verificar/{consecutivo}");

        // Criterio de la historia: solo datos minimos. Ni tercero, ni concepto,
        // ni centro de costo.
        cuerpo.Should().Contain("numeroConsecutivo");
        cuerpo.Should().Contain("valorCOP");
        cuerpo.ToLowerInvariant().Should().NotContain("tercero");
        cuerpo.ToLowerInvariant().Should().NotContain("centrocosto");
        cuerpo.ToLowerInvariant().Should().NotContain("descripcion");
    }

    [Fact]
    public async Task Un_consecutivo_inventado_devuelve_404_y_no_un_error()
    {
        await fabrica.PrepararBaseAsync();
        var anonimo = fabrica.CreateClient();

        var respuesta = await anonimo.GetAsync("/api/recibos/verificar/NO-EXISTE-9999");

        // En un endpoint publico, un numero inventado es un caso normal.
        respuesta.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task El_recibo_verificado_informa_el_valor_del_movimiento()
    {
        var cliente = fabrica.CrearCliente();
        var consecutivo = await CrearComprobanteAsync(cliente);

        var anonimo = fabrica.CreateClient();
        var recibo = await anonimo.GetFromJsonAsync<ReciboVerificado>($"/api/recibos/verificar/{consecutivo}");

        recibo!.ValorCOP.Should().Be(25000m);
        recibo.EsValido.Should().BeTrue();
    }

    private sealed record ReciboVerificado(
        string NumeroConsecutivo,
        DateTime Fecha,
        decimal ValorCOP,
        string Estado,
        bool EsValido);

    private sealed record RespuestaId(Guid Id);
}
