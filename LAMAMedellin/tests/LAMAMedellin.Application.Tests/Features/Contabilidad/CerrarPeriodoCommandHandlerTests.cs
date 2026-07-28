using FluentAssertions;
using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Common.Interfaces.Services;
using LAMAMedellin.Application.Features.Contabilidad.Commands.CerrarPeriodo;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Moq;
using Xunit;

namespace LAMAMedellin.Application.Tests.Features.Contabilidad;

public sealed class CerrarPeriodoCommandHandlerTests
{
    private readonly Mock<IPeriodoContableRepository> _periodoRepositoryMock = new();
    private readonly Mock<IUsuarioActual> _usuarioActualMock = new();

    public CerrarPeriodoCommandHandlerTests()
    {
        _usuarioActualMock.SetupGet(u => u.Identificador).Returns("contador@lamamedellin.org");
    }

    [Fact]
    public async Task CuandoElPeriodoNoExiste_DebeExigirValidacionPrevia()
    {
        _periodoRepositoryMock
            .Setup(r => r.GetPorAnioYMesAsync(2026, 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PeriodoContable?)null);

        var act = async () => await CrearSut().Handle(new CerrarPeriodoCommand(2026, 7), CancellationToken.None);

        await act.Should().ThrowAsync<ExcepcionNegocio>().WithMessage("*validado por tesoreria*");
    }

    [Fact]
    public async Task ConComprobantesEnBorrador_NoDebeCerrar()
    {
        // Un borrador dentro del mes es un movimiento a medio registrar. Cerrar
        // encima lo dejaria en un limbo del que solo se sale con un ajuste.
        var periodo = CrearPeriodoValidado();

        _periodoRepositoryMock
            .Setup(r => r.GetPorAnioYMesAsync(2026, 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(periodo);

        _periodoRepositoryMock
            .Setup(r => r.ContarComprobantesEnBorradorAsync(2026, 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var act = async () => await CrearSut().Handle(new CerrarPeriodoCommand(2026, 7), CancellationToken.None);

        await act.Should().ThrowAsync<ExcepcionNegocio>().WithMessage("*3 comprobante(s) en borrador*");

        periodo.Estado.Should().Be(EstadoPeriodoContable.ValidadoTesoreria);
        _periodoRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CuandoTodoEstaEnOrden_DebeCerrarYRegistrarAlContador()
    {
        var periodo = CrearPeriodoValidado();

        _periodoRepositoryMock
            .Setup(r => r.GetPorAnioYMesAsync(2026, 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(periodo);

        _periodoRepositoryMock
            .Setup(r => r.ContarComprobantesEnBorradorAsync(2026, 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        await CrearSut().Handle(new CerrarPeriodoCommand(2026, 7), CancellationToken.None);

        periodo.Estado.Should().Be(EstadoPeriodoContable.Cerrado);
        periodo.CerradoPor.Should().Be("contador@lamamedellin.org");
        _periodoRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static PeriodoContable CrearPeriodoValidado()
    {
        var periodo = new PeriodoContable(2026, 7);
        periodo.ValidarTesoreria("tesorero@lamamedellin.org");
        return periodo;
    }

    private CerrarPeriodoCommandHandler CrearSut() =>
        new(_periodoRepositoryMock.Object, _usuarioActualMock.Object);
}
