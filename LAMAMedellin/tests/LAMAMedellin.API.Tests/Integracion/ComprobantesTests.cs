using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LAMAMedellin.API.Tests.Infraestructura;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LAMAMedellin.API.Tests.Integracion;

/// <summary>
/// Registro de comprobantes, extremo a extremo.
///
/// Es donde aparecio el 422 que el usuario no podia diagnosticar: 16 de las 24
/// cuentas asentables exigen tercero y el formulario lo pedia como un GUID a
/// mano. Estas pruebas fijan tanto la regla como el mensaje, para que un fallo
/// aqui diga que paso y no solo que fallo.
/// </summary>
public sealed class ComprobantesTests(FabricaApiPruebas fabrica) : IClassFixture<FabricaApiPruebas>
{
    private const string Ruta = "/api/comprobantes";

    private sealed record Contexto(Guid CuentaSinTercero, Guid CuentaConTercero, Guid CentroCosto, Guid MiembroId);

    private async Task<Contexto> PrepararAsync()
    {
        var contexto = await fabrica.PrepararBaseAsync();

        var sinTercero = await contexto.CuentasContables
            .FirstAsync(c => c.PermiteMovimiento && !c.ExigeTercero);

        var conTercero = await contexto.CuentasContables
            .FirstAsync(c => c.PermiteMovimiento && c.ExigeTercero);

        var centroCosto = await contexto.CentrosCosto.FirstAsync();

        var miembro = await contexto.Miembros.FirstOrDefaultAsync();
        miembro.Should().NotBeNull("el sembrador debe dejar al menos un miembro para poder usarlo como tercero");

        return new Contexto(sinTercero.Id, conTercero.Id, centroCosto.Id, miembro!.Id);
    }

    private static object ArmarComprobante(Guid cuentaDebe, Guid cuentaHaber, Guid centroCosto,
        Guid? terceroDebe = null, Guid? terceroHaber = null)
    {
        return new
        {
            fecha = DateTime.UtcNow.Date,
            tipo = (int)TipoComprobante.Diario,
            descripcion = "Comprobante de prueba",
            asientos = new object[]
            {
                new
                {
                    cuentaContableId = cuentaDebe,
                    terceroId = terceroDebe,
                    centroCostoId = centroCosto,
                    debe = 50000m,
                    haber = 0m,
                    referencia = "Linea debito",
                },
                new
                {
                    cuentaContableId = cuentaHaber,
                    terceroId = terceroHaber,
                    centroCostoId = centroCosto,
                    debe = 0m,
                    haber = 50000m,
                    referencia = "Linea credito",
                },
            },
        };
    }

    [Fact]
    public async Task Un_comprobante_cuadrado_con_sus_terceros_se_registra()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente();

        var respuesta = await cliente.PostAsJsonAsync(Ruta,
            ArmarComprobante(c.CuentaConTercero, c.CuentaConTercero, c.CentroCosto, c.MiembroId, c.MiembroId));

        respuesta.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Falta_el_tercero_en_una_cuenta_que_lo_exige_y_el_mensaje_lo_dice()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente();

        var respuesta = await cliente.PostAsJsonAsync(Ruta,
            ArmarComprobante(c.CuentaConTercero, c.CuentaConTercero, c.CentroCosto));

        respuesta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        // Este es el punto: el cuerpo debe traer la razon. Cuando no llegaba, la
        // pantalla mostraba "No fue posible registrar el comprobante" y no habia
        // forma de saber que faltaba.
        var problema = await respuesta.Content.ReadFromJsonAsync<DetalleProblema>();
        problema!.Detail.Should().Contain("tercero");
    }

    [Fact]
    public async Task Un_tercero_que_no_existe_se_rechaza()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente();
        var inventado = Guid.NewGuid();

        var respuesta = await cliente.PostAsJsonAsync(Ruta,
            ArmarComprobante(c.CuentaConTercero, c.CuentaConTercero, c.CentroCosto, inventado, inventado));

        // TerceroId no tiene clave foranea, asi que sin esta comprobacion el
        // asiento quedaba apuntando a nadie y solo se descubria en la exogena.
        respuesta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var problema = await respuesta.Content.ReadFromJsonAsync<DetalleProblema>();
        problema!.Detail.Should().Contain("no corresponde");
    }

    [Fact]
    public async Task Un_comprobante_descuadrado_no_se_registra()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente();

        var descuadrado = new
        {
            fecha = DateTime.UtcNow.Date,
            tipo = (int)TipoComprobante.Diario,
            descripcion = "Descuadrado a proposito",
            asientos = new object[]
            {
                new
                {
                    cuentaContableId = c.CuentaConTercero,
                    terceroId = (Guid?)c.MiembroId,
                    centroCostoId = c.CentroCosto,
                    debe = 50000m,
                    haber = 0m,
                    referencia = "Debito",
                },
                new
                {
                    cuentaContableId = c.CuentaConTercero,
                    terceroId = (Guid?)c.MiembroId,
                    centroCostoId = c.CentroCosto,
                    debe = 0m,
                    haber = 30000m,
                    referencia = "Credito que no cuadra",
                },
            },
        };

        var respuesta = await cliente.PostAsJsonAsync(Ruta, descuadrado);

        // La partida doble es la regla que sostiene todo lo demas.
        respuesta.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Una_cuenta_que_no_admite_movimiento_se_rechaza()
    {
        var contexto = await fabrica.PrepararBaseAsync();
        var c = await PrepararAsync();

        var cuentaGrupo = await contexto.CuentasContables.FirstAsync(x => !x.PermiteMovimiento);
        var cliente = fabrica.CrearCliente();

        var respuesta = await cliente.PostAsJsonAsync(Ruta,
            ArmarComprobante(cuentaGrupo.Id, c.CuentaConTercero, c.CentroCosto, c.MiembroId, c.MiembroId));

        respuesta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var problema = await respuesta.Content.ReadFromJsonAsync<DetalleProblema>();
        problema!.Detail.Should().Contain("movimiento");
    }

    [Fact]
    public async Task Un_centro_de_costo_inexistente_se_rechaza()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente();

        var respuesta = await cliente.PostAsJsonAsync(Ruta,
            ArmarComprobante(c.CuentaConTercero, c.CuentaConTercero, Guid.NewGuid(), c.MiembroId, c.MiembroId));

        respuesta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Todo_error_de_negocio_llega_con_su_detalle()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente();

        var respuesta = await cliente.PostAsJsonAsync(Ruta,
            ArmarComprobante(c.CuentaConTercero, c.CuentaConTercero, c.CentroCosto));

        var problema = await respuesta.Content.ReadFromJsonAsync<DetalleProblema>();

        // El frontend lee `detail`. Si el contrato cambiara, la pantalla
        // volveria a mostrar un mensaje generico sin que nadie se entere.
        problema.Should().NotBeNull();
        problema!.Detail.Should().NotBeNullOrWhiteSpace();
        problema.Title.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Los_comprobantes_se_pueden_listar()
    {
        var c = await PrepararAsync();
        var cliente = fabrica.CrearCliente();

        await cliente.PostAsJsonAsync(Ruta,
            ArmarComprobante(c.CuentaConTercero, c.CuentaConTercero, c.CentroCosto, c.MiembroId, c.MiembroId));

        var listado = await cliente.GetFromJsonAsync<List<ComprobanteResumen>>(Ruta);

        // No habia forma de consultarlos: se creaban y ninguna pantalla podia
        // ofrecerlos para elegir ni descargar su recibo.
        listado.Should().NotBeNull();
        listado!.Should().NotBeEmpty();
        listado.Should().OnlyContain(x => !string.IsNullOrWhiteSpace(x.NumeroConsecutivo));
        listado.Should().Contain(x => x.Total == 50000m);
    }

    private sealed record ComprobanteResumen(
        Guid Id,
        string NumeroConsecutivo,
        DateTime Fecha,
        string TipoComprobante,
        string Descripcion,
        string Estado,
        decimal Total);

    private sealed record DetalleProblema(string? Title, string? Detail, int? Status);
}
