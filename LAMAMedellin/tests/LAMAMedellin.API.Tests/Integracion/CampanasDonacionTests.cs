using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LAMAMedellin.API.Tests.Infraestructura;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LAMAMedellin.API.Tests.Integracion;

/// <summary>
/// Campanas de donacion (historias 2-1 y 2-2).
///
/// Agrupan donaciones bajo un proposito y una ventana de tiempo, que es lo que
/// permite decir cuanto se recaudo de lo que se pretendia.
/// </summary>
public sealed class CampanasDonacionTests(FabricaApiPruebas fabrica) : IClassFixture<FabricaApiPruebas>
{
    private const string Ruta = "/api/donaciones/campanas";

    private static object Campana(string nombre, decimal meta, int diasDesplazamiento = 0) => new
    {
        nombre,
        descripcion = "Convocatoria de prueba",
        metaCOP = meta,
        fechaInicio = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(diasDesplazamiento - 5),
        fechaFin = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(diasDesplazamiento + 30),
    };

    [Fact]
    public async Task Una_campana_nueva_arranca_sin_recaudo()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var creacion = await cliente.PostAsJsonAsync(Ruta, Campana($"Techo digno {Guid.NewGuid():N}"[..24], 5_000_000m));
        creacion.StatusCode.Should().Be(HttpStatusCode.Created);

        var creada = await creacion.Content.ReadFromJsonAsync<RespuestaId>();
        var campanas = await cliente.GetFromJsonAsync<List<CampanaRespuesta>>(Ruta);
        var campana = campanas!.Single(c => c.Id == creada!.Id);

        campana.RecaudadoCOP.Should().Be(0m);
        campana.PorcentajeAvance.Should().Be(0m);
        campana.CantidadDonaciones.Should().Be(0);
        campana.EstaVigente.Should().BeTrue();
    }

    [Fact]
    public async Task La_meta_debe_ser_mayor_a_cero()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var respuesta = await cliente.PostAsJsonAsync(Ruta, Campana("Meta invalida", 0m));

        respuesta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task La_fecha_de_fin_no_puede_ser_anterior_a_la_de_inicio()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var respuesta = await cliente.PostAsJsonAsync(Ruta, new
        {
            nombre = "Ventana invertida",
            descripcion = "Fechas al reves",
            metaCOP = 100000m,
            fechaInicio = DateOnly.FromDateTime(DateTime.UtcNow),
            fechaFin = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-10),
        });

        respuesta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Una_donacion_imputada_suma_al_avance_de_la_campana()
    {
        var contexto = await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var creacion = await cliente.PostAsJsonAsync(Ruta, Campana($"Mercados {Guid.NewGuid():N}"[..22], 1_000_000m));
        var campanaId = (await creacion.Content.ReadFromJsonAsync<RespuestaId>())!.Id;

        var donanteRespuesta = await cliente.PostAsJsonAsync("/api/donaciones/donantes", new
        {
            nombreORazonSocial = "Donante de prueba",
            tipoDocumento = 1,
            numeroDocumento = $"D{Guid.NewGuid():N}"[..10],
            email = "donante@prueba.org",
            tipoPersona = 1,
        });
        donanteRespuesta.StatusCode.Should().Be(HttpStatusCode.Created);
        var donanteId = (await donanteRespuesta.Content.ReadFromJsonAsync<RespuestaId>())!.Id;

        var banco = await contexto.Bancos.FirstAsync(b => b.EsActivo);
        var centro = await contexto.CentrosCosto.FirstAsync();

        var donacion = await cliente.PostAsJsonAsync("/api/donaciones", new
        {
            donanteId,
            montoCOP = 250_000m,
            bancoId = banco.Id,
            centroCostoId = centro.Id,
            medioPago = 1,
            formaDonacion = 1,
            medioPagoODescripcion = "Transferencia",
            campanaDonacionId = campanaId,
        });
        donacion.StatusCode.Should().Be(HttpStatusCode.Created);

        var campanas = await cliente.GetFromJsonAsync<List<CampanaRespuesta>>(Ruta);
        var campana = campanas!.Single(c => c.Id == campanaId);

        campana.RecaudadoCOP.Should().Be(250_000m);
        campana.CantidadDonaciones.Should().Be(1);
        // El avance se calcula, no se guarda: un total almacenado se
        // desincroniza en cuanto alguien corrige una donacion.
        campana.PorcentajeAvance.Should().Be(25m);
    }

    [Fact]
    public async Task Una_campana_cerrada_no_admite_donaciones()
    {
        var contexto = await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var creacion = await cliente.PostAsJsonAsync(Ruta, Campana($"Cerrada {Guid.NewGuid():N}"[..20], 500_000m));
        var campanaId = (await creacion.Content.ReadFromJsonAsync<RespuestaId>())!.Id;

        var cierre = await cliente.PatchAsJsonAsync($"{Ruta}/{campanaId}/estado", new { activa = false });
        cierre.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var donanteRespuesta = await cliente.PostAsJsonAsync("/api/donaciones/donantes", new
        {
            nombreORazonSocial = "Donante tardio",
            tipoDocumento = 1,
            numeroDocumento = $"T{Guid.NewGuid():N}"[..10],
            email = "tardio@prueba.org",
            tipoPersona = 1,
        });
        var donanteId = (await donanteRespuesta.Content.ReadFromJsonAsync<RespuestaId>())!.Id;

        var banco = await contexto.Bancos.FirstAsync(b => b.EsActivo);
        var centro = await contexto.CentrosCosto.FirstAsync();

        var donacion = await cliente.PostAsJsonAsync("/api/donaciones", new
        {
            donanteId,
            montoCOP = 100_000m,
            bancoId = banco.Id,
            centroCostoId = centro.Id,
            medioPago = 1,
            formaDonacion = 1,
            medioPagoODescripcion = "Transferencia",
            campanaDonacionId = campanaId,
        });

        donacion.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Una_donacion_sin_campana_sigue_siendo_valida()
    {
        var contexto = await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var donanteRespuesta = await cliente.PostAsJsonAsync("/api/donaciones/donantes", new
        {
            nombreORazonSocial = "Donante espontaneo",
            tipoDocumento = 1,
            numeroDocumento = $"E{Guid.NewGuid():N}"[..10],
            email = "espontaneo@prueba.org",
            tipoPersona = 1,
        });
        var donanteId = (await donanteRespuesta.Content.ReadFromJsonAsync<RespuestaId>())!.Id;

        var banco = await contexto.Bancos.FirstAsync(b => b.EsActivo);
        var centro = await contexto.CentrosCosto.FirstAsync();

        var donacion = await cliente.PostAsJsonAsync("/api/donaciones", new
        {
            donanteId,
            montoCOP = 75_000m,
            bancoId = banco.Id,
            centroCostoId = centro.Id,
            medioPago = 1,
            formaDonacion = 1,
            medioPagoODescripcion = "Transferencia",
        });

        // No toda donacion responde a una convocatoria.
        donacion.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private sealed record CampanaRespuesta(
        Guid Id,
        string Nombre,
        decimal MetaCOP,
        decimal RecaudadoCOP,
        decimal PorcentajeAvance,
        int CantidadDonaciones,
        bool EstaActiva,
        bool EstaVigente);

    private sealed record RespuestaId(Guid Id);
}
