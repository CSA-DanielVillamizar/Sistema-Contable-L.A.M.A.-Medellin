using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LAMAMedellin.API.Tests.Infraestructura;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LAMAMedellin.API.Tests.Integracion;

/// <summary>
/// Actividades de proyecto y rendicion de cuentas (historias 3-1 y 3-4).
///
/// Sin actividades, un proyecto es un nombre con un presupuesto y una fecha, y
/// no hay forma de decir cuanto se ha avanzado.
/// </summary>
public sealed class ProyectosRendicionTests(FabricaApiPruebas fabrica) : IClassFixture<FabricaApiPruebas>
{
    private async Task<Guid> CrearProyectoAsync(HttpClient cliente, decimal presupuesto)
    {
        var contexto = await fabrica.PrepararBaseAsync();
        var centro = await contexto.CentrosCosto.FirstAsync();

        var respuesta = await cliente.PostAsJsonAsync("/api/proyectos", new
        {
            centroCostoId = centro.Id,
            nombre = $"Proyecto {Guid.NewGuid():N}"[..20],
            descripcion = "Proyecto social de prueba",
            fechaInicio = DateTime.UtcNow.Date,
            fechaFin = DateTime.UtcNow.Date.AddMonths(6),
            presupuestoEstimado = presupuesto,
            estado = 1,
        });

        respuesta.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await respuesta.Content.ReadFromJsonAsync<RespuestaId>())!.Id;
    }

    private static object Actividad(string nombre, decimal presupuesto, int diasFin = 30) => new
    {
        nombre,
        descripcion = "Actividad de prueba",
        fechaInicioPlanificada = DateOnly.FromDateTime(DateTime.UtcNow),
        fechaFinPlanificada = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(diasFin),
        presupuestoAsignado = presupuesto,
        responsable = "Coordinador",
    };

    [Fact]
    public async Task Una_actividad_nueva_nace_planificada()
    {
        var cliente = fabrica.CrearCliente("Operador");
        var proyectoId = await CrearProyectoAsync(cliente, 10_000_000m);

        var creacion = await cliente.PostAsJsonAsync(
            $"/api/proyectos/{proyectoId}/actividades",
            Actividad("Compra de mercados", 2_000_000m));

        creacion.StatusCode.Should().Be(HttpStatusCode.Created);

        var actividades = await cliente.GetFromJsonAsync<List<ActividadRespuesta>>(
            $"/api/proyectos/{proyectoId}/actividades");

        actividades!.Should().ContainSingle();
        actividades[0].NombreEstado.Should().Be("Planificada");
        actividades[0].EstaVencida.Should().BeFalse();
    }

    [Fact]
    public async Task Una_actividad_con_fecha_pasada_sin_completar_sale_vencida()
    {
        var cliente = fabrica.CrearCliente("Operador");
        var proyectoId = await CrearProyectoAsync(cliente, 5_000_000m);

        await cliente.PostAsJsonAsync($"/api/proyectos/{proyectoId}/actividades", new
        {
            nombre = "Entrega atrasada",
            descripcion = "Debio terminar hace tiempo",
            fechaInicioPlanificada = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-60),
            fechaFinPlanificada = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-30),
            presupuestoAsignado = 500_000m,
            responsable = "Coordinador",
        });

        var actividades = await cliente.GetFromJsonAsync<List<ActividadRespuesta>>(
            $"/api/proyectos/{proyectoId}/actividades");

        // Es la senal que el coordinador necesita, no la fecha en si.
        actividades!.Single().EstaVencida.Should().BeTrue();
    }

    [Fact]
    public async Task Una_actividad_completada_ya_no_cambia_de_estado()
    {
        var cliente = fabrica.CrearCliente("Operador");
        var proyectoId = await CrearProyectoAsync(cliente, 3_000_000m);

        var creacion = await cliente.PostAsJsonAsync(
            $"/api/proyectos/{proyectoId}/actividades",
            Actividad("Taller de formacion", 300_000m));
        var actividadId = (await creacion.Content.ReadFromJsonAsync<RespuestaId>())!.Id;

        var completar = await cliente.PatchAsJsonAsync(
            $"/api/proyectos/actividades/{actividadId}/estado", new { estado = 3 });
        completar.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var reabrir = await cliente.PatchAsJsonAsync(
            $"/api/proyectos/actividades/{actividadId}/estado", new { estado = 2 });

        // El estado final es parte de lo que se rinde.
        reabrir.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task La_rendicion_reporta_el_avance_de_actividades()
    {
        var cliente = fabrica.CrearCliente("Operador");
        var proyectoId = await CrearProyectoAsync(cliente, 8_000_000m);

        var primera = await cliente.PostAsJsonAsync(
            $"/api/proyectos/{proyectoId}/actividades", Actividad("Actividad uno", 1_000_000m));
        var primeraId = (await primera.Content.ReadFromJsonAsync<RespuestaId>())!.Id;

        await cliente.PostAsJsonAsync(
            $"/api/proyectos/{proyectoId}/actividades", Actividad("Actividad dos", 2_000_000m));

        await cliente.PatchAsJsonAsync($"/api/proyectos/actividades/{primeraId}/estado", new { estado = 3 });

        var rendicion = await cliente.GetFromJsonAsync<List<Rendicion>>(
            $"/api/proyectos/rendicion?proyectoSocialId={proyectoId}");

        var informe = rendicion!.Single();
        informe.TotalActividades.Should().Be(2);
        informe.ActividadesCompletadas.Should().Be(1);
        informe.PorcentajeAvanceActividades.Should().Be(50m);
        informe.PresupuestoAsignadoAActividades.Should().Be(3_000_000m);
    }

    [Fact]
    public async Task Una_actividad_cancelada_no_penaliza_el_avance()
    {
        var cliente = fabrica.CrearCliente("Operador");
        var proyectoId = await CrearProyectoAsync(cliente, 4_000_000m);

        var completada = await cliente.PostAsJsonAsync(
            $"/api/proyectos/{proyectoId}/actividades", Actividad("Se hizo", 500_000m));
        var completadaId = (await completada.Content.ReadFromJsonAsync<RespuestaId>())!.Id;

        var cancelada = await cliente.PostAsJsonAsync(
            $"/api/proyectos/{proyectoId}/actividades", Actividad("Se descarto", 500_000m));
        var canceladaId = (await cancelada.Content.ReadFromJsonAsync<RespuestaId>())!.Id;

        await cliente.PatchAsJsonAsync($"/api/proyectos/actividades/{completadaId}/estado", new { estado = 3 });
        await cliente.PatchAsJsonAsync($"/api/proyectos/actividades/{canceladaId}/estado", new { estado = 4 });

        var rendicion = await cliente.GetFromJsonAsync<List<Rendicion>>(
            $"/api/proyectos/rendicion?proyectoSocialId={proyectoId}");

        // Exigir completar algo que se decidio no hacer dejaria el proyecto
        // siempre incompleto.
        rendicion!.Single().PorcentajeAvanceActividades.Should().Be(100m);
    }

    [Fact]
    public async Task La_rendicion_no_expone_datos_de_beneficiarios()
    {
        var cliente = fabrica.CrearCliente("Operador");
        var proyectoId = await CrearProyectoAsync(cliente, 1_000_000m);

        var cuerpo = await cliente.GetStringAsync($"/api/proyectos/rendicion?proyectoSocialId={proyectoId}");

        // La rendicion es publica por naturaleza; la PII esta restringida por
        // rol (historia 3-3). Solo debe viajar el conteo.
        cuerpo.Should().Contain("totalBeneficiarios");
        cuerpo.ToLowerInvariant().Should().NotContain("documento");
        cuerpo.ToLowerInvariant().Should().NotContain("consentimiento");
    }

    [Fact]
    public async Task Sin_movimientos_contables_lo_ejecutado_es_cero()
    {
        var cliente = fabrica.CrearCliente("Operador");
        var proyectoId = await CrearProyectoAsync(cliente, 6_000_000m);

        var rendicion = await cliente.GetFromJsonAsync<List<Rendicion>>(
            $"/api/proyectos/rendicion?proyectoSocialId={proyectoId}");

        var informe = rendicion!.Single();

        // Lo ejecutado sale del libro, no de un campo que alguien mantenga.
        informe.EjecutadoCOP.Should().Be(0m);
        informe.DisponibleCOP.Should().Be(6_000_000m);
        informe.PorcentajeEjecucion.Should().Be(0m);
    }

    private sealed record ActividadRespuesta(
        Guid Id,
        string Nombre,
        decimal PresupuestoAsignado,
        int Estado,
        string NombreEstado,
        bool EstaVencida);

    private sealed record Rendicion(
        Guid ProyectoSocialId,
        string Nombre,
        decimal PresupuestoEstimado,
        decimal PresupuestoAsignadoAActividades,
        decimal EjecutadoCOP,
        decimal DisponibleCOP,
        decimal PorcentajeEjecucion,
        int TotalActividades,
        int ActividadesCompletadas,
        int ActividadesVencidas,
        decimal PorcentajeAvanceActividades,
        int TotalBeneficiarios);

    private sealed record RespuestaId(Guid Id);
}
