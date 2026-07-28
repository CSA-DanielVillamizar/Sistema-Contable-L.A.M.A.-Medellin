using FluentAssertions;
using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Xunit;

namespace LAMAMedellin.Application.Tests.Domain;

public sealed class PeriodoContableTests
{
    [Fact]
    public void NuevoPeriodo_DebeNacerAbierto()
    {
        var periodo = new PeriodoContable(2026, 7);

        periodo.Estado.Should().Be(EstadoPeriodoContable.Abierto);
        periodo.EstaCerrado.Should().BeFalse();
    }

    [Theory]
    [InlineData(2026, 0)]
    [InlineData(2026, 13)]
    [InlineData(1999, 5)]
    public void PeriodoInvalido_DebeRechazarse(int anio, int mes)
    {
        var act = () => new PeriodoContable(anio, mes);

        act.Should().Throw<ReglaNegocioException>();
    }

    [Fact]
    public void NoSePuedeCerrarSinValidacionDeTesoreria()
    {
        // Es el control de segregacion de funciones: el Contador no puede
        // cerrar un mes que el Tesorero no reviso.
        var periodo = new PeriodoContable(2026, 7);

        var act = () => periodo.Cerrar("contador@lamamedellin.org");

        act.Should().Throw<ReglaNegocioException>()
            .WithMessage("*validado por tesoreria*");
    }

    [Fact]
    public void FlujoCompleto_DebeRegistrarQuienValidoYQuienCerro()
    {
        var periodo = new PeriodoContable(2026, 7);

        periodo.ValidarTesoreria("tesorero@lamamedellin.org");
        periodo.Estado.Should().Be(EstadoPeriodoContable.ValidadoTesoreria);

        periodo.Cerrar("contador@lamamedellin.org");

        periodo.Estado.Should().Be(EstadoPeriodoContable.Cerrado);
        periodo.EstaCerrado.Should().BeTrue();

        // Los dos hechos quedan por separado: UpdatedBy solo conservaria el ultimo.
        periodo.ValidadoPor.Should().Be("tesorero@lamamedellin.org");
        periodo.CerradoPor.Should().Be("contador@lamamedellin.org");
        periodo.FechaValidacionTesoreria.Should().NotBeNull();
        periodo.FechaCierre.Should().NotBeNull();
    }

    [Fact]
    public void PeriodoCerrado_NoSePuedeValidarNiCerrarDeNuevo()
    {
        var periodo = new PeriodoContable(2026, 7);
        periodo.ValidarTesoreria("tesorero@lamamedellin.org");
        periodo.Cerrar("contador@lamamedellin.org");

        var validar = () => periodo.ValidarTesoreria("tesorero@lamamedellin.org");
        var cerrar = () => periodo.Cerrar("contador@lamamedellin.org");

        validar.Should().Throw<ReglaNegocioException>();
        cerrar.Should().Throw<ReglaNegocioException>();
    }

    [Fact]
    public void NoSePuedeValidarDosVeces()
    {
        var periodo = new PeriodoContable(2026, 7);
        periodo.ValidarTesoreria("tesorero@lamamedellin.org");

        var act = () => periodo.ValidarTesoreria("otro@lamamedellin.org");

        act.Should().Throw<ReglaNegocioException>();
    }

    [Fact]
    public void Contiene_DebeReconocerSoloSuPropioMes()
    {
        var periodo = new PeriodoContable(2026, 7);

        periodo.Contiene(new DateTime(2026, 7, 31, 23, 59, 0, DateTimeKind.Utc)).Should().BeTrue();
        periodo.Contiene(new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc)).Should().BeFalse();
        periodo.Contiene(new DateTime(2025, 7, 15, 0, 0, 0, DateTimeKind.Utc)).Should().BeFalse();
    }
}
