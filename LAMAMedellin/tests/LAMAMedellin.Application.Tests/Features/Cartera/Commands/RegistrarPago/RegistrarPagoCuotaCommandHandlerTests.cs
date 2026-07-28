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

        // Comportamiento actual: el handler aplica el pago sobre la entidad ANTES de
        // verificar que exista banco, por lo que la CxC queda mutada pese al fallo.
        // No se persiste porque no se llama SaveChanges, pero la entidad rastreada
        // queda inconsistente. Ver pendiente de reordenar el handler.
        cxc.SaldoPendiente.Should().Be(50_000m);
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
        cxc.SaldoPendiente.Should().Be(0m);
        cxc.Estado.Should().Be(EstadoCuentaPorCobrar.Pagada);
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
        var fechaEmision = new DateOnly(2026, 2, 1);

        var cxc = new CuentaPorCobrar(
            miembroId: Guid.NewGuid(),
            conceptoCobroId: Guid.NewGuid(),
            fechaEmision: fechaEmision,
            fechaVencimiento: fechaEmision.AddMonths(1).AddDays(-1),
            valorTotal: saldoPendiente)
        {
            Id = id
        };

        return cxc;
    }
}
