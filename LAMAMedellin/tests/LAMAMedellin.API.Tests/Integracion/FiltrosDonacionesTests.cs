using System.Net.Http.Json;
using FluentAssertions;
using LAMAMedellin.API.Tests.Infraestructura;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LAMAMedellin.API.Tests.Integracion;

/// <summary>
/// Filtros y contenido del listado de donaciones (historia 2-4).
///
/// La consulta no admitia ningun filtro: la pantalla traia el historico
/// completo y acotaba en el navegador. Estas pruebas fijan que filtrar ocurra
/// del lado del servidor y que cada criterio se aplique de verdad; una
/// consulta que ignora un parametro devuelve datos de mas sin fallar, que es
/// la forma mas silenciosa de equivocarse.
/// </summary>
public sealed class FiltrosDonacionesTests(FabricaApiPruebas fabrica) : IClassFixture<FabricaApiPruebas>
{
    private const string Ruta = "/api/donaciones";

    [Fact]
    public async Task El_listado_trae_el_nombre_del_banco_y_del_centro_de_costo()
    {
        var contexto = await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var banco = await contexto.Bancos.FirstAsync(b => b.EsActivo);
        var centro = await contexto.CentrosCosto.FirstAsync();
        await RegistrarDonacion(cliente, "Nombres", 120_000m, banco.Id, centro.Id);

        var donaciones = await cliente.GetFromJsonAsync<List<DonacionRespuesta>>(Ruta);

        // El contrato solo traia los Guid, de modo que la pantalla mostraba
        // vacio donde debia ir el banco.
        var registrada = donaciones!.Single(d => d.NombreDonante == "Nombres");
        registrada.Banco.Should().Be(banco.Nombre);
        registrada.CentroCosto.Should().Be(centro.Nombre);
    }

    [Fact]
    public async Task Filtrar_por_donante_deja_fuera_a_los_demas()
    {
        var contexto = await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var banco = await contexto.Bancos.FirstAsync(b => b.EsActivo);
        var centro = await contexto.CentrosCosto.FirstAsync();

        var buscado = await RegistrarDonacion(cliente, "Buscado", 200_000m, banco.Id, centro.Id);
        await RegistrarDonacion(cliente, "Ignorado", 300_000m, banco.Id, centro.Id);

        var donaciones = await cliente.GetFromJsonAsync<List<DonacionRespuesta>>(
            $"{Ruta}?donanteId={buscado}");

        donaciones!.Should().OnlyContain(d => d.DonanteId == buscado);
        donaciones.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Filtrar_por_certificado_separa_emitidos_de_pendientes()
    {
        var contexto = await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var banco = await contexto.Bancos.FirstAsync(b => b.EsActivo);
        var centro = await contexto.CentrosCosto.FirstAsync();
        var donanteId = await RegistrarDonacion(cliente, "Certificado", 90_000m, banco.Id, centro.Id);

        var pendientes = await cliente.GetFromJsonAsync<List<DonacionRespuesta>>(
            $"{Ruta}?certificadoEmitido=false");
        var emitidas = await cliente.GetFromJsonAsync<List<DonacionRespuesta>>(
            $"{Ruta}?certificadoEmitido=true");

        // La donacion recien registrada aun no tiene certificado, asi que debe
        // aparecer de un lado y no del otro. Comprobar solo que "emitidas"
        // contenga emitidas no prueba nada cuando esa lista viene vacia.
        pendientes!.Should().Contain(d => d.DonanteId == donanteId);
        pendientes.Should().OnlyContain(d => !d.CertificadoEmitido);
        emitidas!.Should().NotContain(d => d.DonanteId == donanteId);
    }

    [Fact]
    public async Task Un_rango_de_fechas_anterior_al_registro_no_devuelve_nada()
    {
        var contexto = await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var banco = await contexto.Bancos.FirstAsync(b => b.EsActivo);
        var centro = await contexto.CentrosCosto.FirstAsync();
        await RegistrarDonacion(cliente, "Fuera de rango", 60_000m, banco.Id, centro.Id);

        var anteayer = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-2);
        var ayer = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);

        var donaciones = await cliente.GetFromJsonAsync<List<DonacionRespuesta>>(
            $"{Ruta}?desde={anteayer:yyyy-MM-dd}&hasta={ayer:yyyy-MM-dd}");

        donaciones.Should().BeEmpty();
    }

    [Fact]
    public async Task Sin_filtros_el_listado_devuelve_todo()
    {
        var contexto = await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var banco = await contexto.Bancos.FirstAsync(b => b.EsActivo);
        var centro = await contexto.CentrosCosto.FirstAsync();
        await RegistrarDonacion(cliente, "Primera", 10_000m, banco.Id, centro.Id);
        await RegistrarDonacion(cliente, "Segunda", 20_000m, banco.Id, centro.Id);

        var donaciones = await cliente.GetFromJsonAsync<List<DonacionRespuesta>>(Ruta);

        donaciones!.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    /// <summary>Crea donante y donacion, y devuelve el identificador del donante.</summary>
    private static async Task<Guid> RegistrarDonacion(
        HttpClient cliente,
        string nombreDonante,
        decimal monto,
        Guid bancoId,
        Guid centroCostoId)
    {
        var donante = await cliente.PostAsJsonAsync("/api/donaciones/donantes", new
        {
            nombreORazonSocial = nombreDonante,
            tipoDocumento = 1,
            numeroDocumento = $"F{Guid.NewGuid():N}"[..10],
            email = "filtros@prueba.org",
            tipoPersona = 1,
        });

        var donanteId = (await donante.Content.ReadFromJsonAsync<RespuestaId>())!.Id;

        var donacion = await cliente.PostAsJsonAsync(Ruta, new
        {
            donanteId,
            montoCOP = monto,
            bancoId,
            centroCostoId,
            medioPago = 1,
            formaDonacion = 1,
            medioPagoODescripcion = "Transferencia",
        });

        donacion.IsSuccessStatusCode.Should().BeTrue();

        return donanteId;
    }

    private sealed record DonacionRespuesta(
        Guid Id,
        Guid DonanteId,
        string NombreDonante,
        decimal MontoCOP,
        DateTime Fecha,
        Guid BancoId,
        string Banco,
        Guid CentroCostoId,
        string CentroCosto,
        bool CertificadoEmitido);

    private sealed record RespuestaId(Guid Id);
}
