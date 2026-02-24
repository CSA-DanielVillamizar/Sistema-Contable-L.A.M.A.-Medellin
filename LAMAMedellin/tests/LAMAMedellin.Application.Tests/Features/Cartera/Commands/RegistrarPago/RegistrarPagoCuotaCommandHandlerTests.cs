using FluentAssertions;
using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Features.Cartera.Commands.RegistrarPago;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;
using Moq;
using Xunit;

namespace LAMAMedellin.Application.Tests.Features.Cartera.Commands.RegistrarPago;

public sealed class RegistrarPagoCuotaCommandHandlerTests
{
    private readonly Mock<ICuentaPorCobrarRepository> _cuentaPorCobrarRepositoryMock = new();
    private readonly Mock<IBancoRepository> _bancoRepositoryMock = new();

    [Fact]
    public async Task Handle_CuandoCuentaPorCobrarNoExiste_DebeLanzarExcepcionNegocio()
    {
        var command = new RegistrarPagoCuotaCommand(Guid.NewGuid(), 100_000m);
        var sut = BuildSut();

        _cuentaPorCobrarRepositoryMock
            .Setup(r => r.GetByIdAsync(command.CuentaPorCobrarId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CuentaPorCobrar?)null);

        Func<Task> act = async () => await sut.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ExcepcionNegocio>()
            .WithMessage("La cuenta por cobrar indicada no existe.");

        _bancoRepositoryMock.Verify(r => r.GetDefaultAsync(It.IsAny<CancellationToken>()), Times.Never);
        _cuentaPorCobrarRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CuandoNoHayBancoConfigurado_DebeLanzarExcepcionNegocio()
    {
        var command = new RegistrarPagoCuotaCommand(Guid.NewGuid(), 50_000m);
        var sut = BuildSut();
        var cxc = CrearCuentaPorCobrarConSaldo(command.CuentaPorCobrarId, 100_000m);

        _cuentaPorCobrarRepositoryMock
            .Setup(r => r.GetByIdAsync(command.CuentaPorCobrarId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cxc);

        _bancoRepositoryMock
            .Setup(r => r.GetDefaultAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((Banco?)null);

        Func<Task> act = async () => await sut.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ExcepcionNegocio>()
            .WithMessage("No hay bancos configurados para registrar el pago.");

        cxc.SaldoPendienteCOP.Should().Be(50_000m);
        _cuentaPorCobrarRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CuandoDatosSonValidos_DebeAplicarAbonoIngresoYGuardarCambios()
    {
        var command = new RegistrarPagoCuotaCommand(Guid.NewGuid(), 100_000m);
        var sut = BuildSut();
        var cxc = CrearCuentaPorCobrarConSaldo(command.CuentaPorCobrarId, 100_000m);
        var banco = new Banco("Bancolombia Ahorros", 1_000_000m);

        _cuentaPorCobrarRepositoryMock
            .Setup(r => r.GetByIdAsync(command.CuentaPorCobrarId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cxc);

        _bancoRepositoryMock
            .Setup(r => r.GetDefaultAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(banco);

        var result = await sut.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
        cxc.SaldoPendienteCOP.Should().Be(0m);
        cxc.Estado.Should().Be(EstadoCuentaPorCobrar.Pagado);
        banco.SaldoActual.Should().Be(1_100_000m);

        _cuentaPorCobrarRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private RegistrarPagoCuotaCommandHandler BuildSut()
    {
        return new RegistrarPagoCuotaCommandHandler(
            _cuentaPorCobrarRepositoryMock.Object,
            _bancoRepositoryMock.Object);
    }

    private static CuentaPorCobrar CrearCuentaPorCobrarConSaldo(Guid id, decimal saldoPendiente)
    {
        var cxc = new CuentaPorCobrar(Guid.NewGuid(), "2026-02", saldoPendiente)
        {
            Id = id
        };

        return cxc;
    }
}
