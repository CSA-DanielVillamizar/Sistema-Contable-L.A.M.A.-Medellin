using FluentAssertions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Features.Contabilidad.Queries.GetCatalogoCuentas;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Moq;
using Xunit;

namespace LAMAMedellin.Application.Tests.Features.Contabilidad.Queries;

public sealed class GetCatalogoCuentasQueryHandlerTests
{
    private readonly Mock<ICuentaContableRepository> _repositoryMock = new();

    [Fact]
    public async Task Handle_CuandoSoloAsentablesFalse_DebeLlamarGetAllAsync()
    {
        var cuentas = new List<CuentaContable>
        {
            new("3", "PATRIMONIO INSTITUCIONAL", NaturalezaCuenta.Credito, false, false),
            new("410505", "Cuotas de Afiliación (Nuevos)", NaturalezaCuenta.Credito, true, true),
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentas);

        var handler = new GetCatalogoCuentasQueryHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetCatalogoCuentasQuery(SoloAsentables: false), CancellationToken.None);

        result.Should().HaveCount(2);
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.GetAsentablesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CuandoSoloAsentablesTrue_DebeLlamarGetAsentablesAsync()
    {
        var cuentas = new List<CuentaContable>
        {
            new("410505", "Cuotas de Afiliación (Nuevos)", NaturalezaCuenta.Credito, true, true),
            new("410510", "Cuotas de Sostenimiento (Mensualidad)", NaturalezaCuenta.Credito, true, true),
        };

        _repositoryMock
            .Setup(r => r.GetAsentablesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentas);

        var handler = new GetCatalogoCuentasQueryHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetCatalogoCuentasQuery(SoloAsentables: true), CancellationToken.None);

        result.Should().HaveCount(2);
        _repositoryMock.Verify(r => r.GetAsentablesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DebeMapearCamposCorrectamenteAlDto()
    {
        var padreId = Guid.NewGuid();
        var cuenta = new CuentaContable("410505", "Cuotas de Afiliación (Nuevos)", NaturalezaCuenta.Credito, true, true);
        cuenta.EstablecerPadre(padreId);

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([cuenta]);

        var handler = new GetCatalogoCuentasQueryHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetCatalogoCuentasQuery(), CancellationToken.None);

        var dto = result.Single();
        dto.Codigo.Should().Be("410505");
        dto.Descripcion.Should().Be("Cuotas de Afiliación (Nuevos)");
        dto.Nivel.Should().Be((int)NivelCuenta.Subcuenta);
        dto.NivelNombre.Should().Be("Subcuenta");
        dto.NaturalezaNombre.Should().Be("Credito");
        dto.PermiteMovimiento.Should().BeTrue();
        dto.ExigeTercero.Should().BeTrue();
        dto.CuentaPadreId.Should().Be(padreId);
    }

    [Fact]
    public async Task Handle_CuandoRepositorioRetornaVacio_DebeRetornarListaVacia()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var handler = new GetCatalogoCuentasQueryHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetCatalogoCuentasQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
