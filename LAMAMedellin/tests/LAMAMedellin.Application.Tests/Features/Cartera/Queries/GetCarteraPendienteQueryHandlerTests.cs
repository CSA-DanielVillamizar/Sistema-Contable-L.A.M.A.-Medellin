using FluentAssertions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Features.Cartera.Queries.GetCarteraPendiente;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Moq;
using Xunit;

namespace LAMAMedellin.Application.Tests.Features.Cartera.Queries;

public sealed class GetCarteraPendienteQueryHandlerTests
{
    private readonly Mock<ICuentaPorCobrarRepository> _cuentaPorCobrarRepositoryMock = new();

    [Fact]
    public async Task Handle_DebeIncluirCuentasConAbonoParcial()
    {
        // Una cuenta con abono parcial sigue debiendo dinero y debe aparecer en la
        // cartera pendiente. Filtrar por Estado == Pendiente la dejaba fuera.
        var conAbonoParcial = CrearCuentaPorCobrar("Ana", "Zapata", 100_000m);
        conAbonoParcial.AplicarPago(40_000m);
        conAbonoParcial.Estado.Should().Be(EstadoCuentaPorCobrar.PagadaParcial);

        var sinAbonos = CrearCuentaPorCobrar("Bruno", "Perez", 80_000m);

        _cuentaPorCobrarRepositoryMock
            .Setup(r => r.GetPendientesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([conAbonoParcial, sinAbonos]);

        var sut = new GetCarteraPendienteQueryHandler(_cuentaPorCobrarRepositoryMock.Object);

        var resultado = await sut.Handle(new GetCarteraPendienteQuery(), CancellationToken.None);

        resultado.Should().HaveCount(2);
        resultado.Should().ContainSingle(x => x.NombreMiembro == "Ana Zapata")
            .Which.SaldoPendiente.Should().Be(60_000m);
    }

    [Fact]
    public async Task Handle_DebeConsultarPendientesSinFiltrarPorEstado()
    {
        _cuentaPorCobrarRepositoryMock
            .Setup(r => r.GetPendientesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var sut = new GetCarteraPendienteQueryHandler(_cuentaPorCobrarRepositoryMock.Object);

        await sut.Handle(new GetCarteraPendienteQuery(), CancellationToken.None);

        _cuentaPorCobrarRepositoryMock.Verify(
            r => r.GetPendientesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static CuentaPorCobrar CrearCuentaPorCobrar(string nombres, string apellidos, decimal valorTotal)
    {
        var fechaEmision = new DateOnly(2026, 2, 1);

        var cuenta = new CuentaPorCobrar(
            miembroId: Guid.NewGuid(),
            conceptoCobroId: Guid.NewGuid(),
            periodo: "2026-02",
            fechaEmision: fechaEmision,
            fechaVencimiento: fechaEmision.AddMonths(1).AddDays(-1),
            valorTotal: valorTotal);

        // El handler proyecta el nombre desde la navegacion Miembro.
        typeof(CuentaPorCobrar)
            .GetProperty(nameof(CuentaPorCobrar.Miembro))!
            .SetValue(cuenta, CrearMiembro(nombres, apellidos));

        return cuenta;
    }

    private static Miembro CrearMiembro(string nombres, string apellidos) =>
        new(
            documentoIdentidad: $"DOC-{nombres}",
            nombres: nombres,
            apellidos: apellidos,
            apodo: nombres,
            fechaIngreso: new DateOnly(2025, 1, 1),
            tipoSangre: GrupoSanguineo.O_Positivo,
            nombreContactoEmergencia: "Contacto",
            telefonoContactoEmergencia: "3000000000",
            marcaMoto: "Harley-Davidson",
            modeloMoto: "Softail",
            cilindraje: 883,
            placa: "LAM001");
}
