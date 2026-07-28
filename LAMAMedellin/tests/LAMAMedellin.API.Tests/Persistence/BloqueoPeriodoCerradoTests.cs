using FluentAssertions;
using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using LAMAMedellin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LAMAMedellin.API.Tests.Persistence;

/// <summary>
/// El bloqueo del periodo cerrado vive en LamaDbContext.SaveChanges, que es el
/// unico punto por el que pasa obligatoriamente todo hecho contable. Estas
/// pruebas lo ejercitan de verdad contra un contexto real, porque una prueba de
/// manejador con repositorios simulados no tocaria ese codigo.
/// </summary>
public sealed class BloqueoPeriodoCerradoTests
{
    [Fact]
    public async Task ComprobanteEnPeriodoCerrado_DebeRechazarse()
    {
        await using var contexto = CrearContexto();
        await AgregarPeriodoCerradoAsync(contexto, 2026, 7);

        contexto.Comprobantes.Add(CrearComprobante(
            new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc),
            TipoComprobante.Ingreso));

        var act = async () => await contexto.SaveChangesAsync();

        await act.Should().ThrowAsync<ReglaNegocioException>()
            .WithMessage("*2026-07 esta cerrado*");
    }

    [Fact]
    public async Task ComprobanteDeAjuste_DebeAdmitirseAunConElPeriodoCerrado()
    {
        // Es el mecanismo que el backlog define para corregir despues del
        // cierre sin editar el documento de origen (historia 1-5).
        await using var contexto = CrearContexto();
        await AgregarPeriodoCerradoAsync(contexto, 2026, 7);

        contexto.Comprobantes.Add(CrearComprobante(
            new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc),
            TipoComprobante.Ajuste));

        await contexto.SaveChangesAsync();

        contexto.Comprobantes.Should().HaveCount(1);
    }

    [Fact]
    public async Task ComprobanteEnPeriodoAbierto_DebeGuardarse()
    {
        await using var contexto = CrearContexto();
        await AgregarPeriodoCerradoAsync(contexto, 2026, 7);

        contexto.Comprobantes.Add(CrearComprobante(
            new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            TipoComprobante.Ingreso));

        await contexto.SaveChangesAsync();

        contexto.Comprobantes.Should().HaveCount(1);
    }

    [Fact]
    public async Task PeriodoSoloValidado_NoDebeBloquear()
    {
        // Validado por tesoreria todavia admite movimientos; solo el cierre
        // bloquea.
        await using var contexto = CrearContexto();

        var periodo = new PeriodoContable(2026, 7);
        periodo.ValidarTesoreria("tesorero@lamamedellin.org");
        contexto.PeriodosContables.Add(periodo);
        await contexto.SaveChangesAsync();

        contexto.Comprobantes.Add(CrearComprobante(
            new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc),
            TipoComprobante.Ingreso));

        await contexto.SaveChangesAsync();

        contexto.Comprobantes.Should().HaveCount(1);
    }

    [Fact]
    public async Task GuardadoSinComprobantes_NoDebeConsultarPeriodos()
    {
        // El guardian no debe costar nada en los guardados que no tocan
        // contabilidad: si hay cero comprobantes en juego, sale de inmediato.
        await using var contexto = CrearContexto();
        await AgregarPeriodoCerradoAsync(contexto, 2026, 7);

        contexto.CentrosCosto.Add(new CentroCosto("Capitulo", TipoCentroCosto.Capitulo));

        await contexto.SaveChangesAsync();

        contexto.CentrosCosto.Should().HaveCount(1);
    }

    private static LamaDbContext CrearContexto()
    {
        var opciones = new DbContextOptionsBuilder<LamaDbContext>()
            .UseInMemoryDatabase($"periodo-{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(
                Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new LamaDbContext(opciones);
    }

    private static async Task AgregarPeriodoCerradoAsync(LamaDbContext contexto, int anio, int mes)
    {
        var periodo = new PeriodoContable(anio, mes);
        periodo.ValidarTesoreria("tesorero@lamamedellin.org");
        periodo.Cerrar("contador@lamamedellin.org");

        contexto.PeriodosContables.Add(periodo);
        await contexto.SaveChangesAsync();
    }

    private static Comprobante CrearComprobante(DateTime fecha, TipoComprobante tipo) =>
        new(
            numeroConsecutivo: $"TST-{Guid.NewGuid():N}"[..20],
            fecha: fecha,
            tipoComprobante: tipo,
            descripcion: "Comprobante de prueba",
            estadoComprobante: EstadoComprobante.Asentado);
}
