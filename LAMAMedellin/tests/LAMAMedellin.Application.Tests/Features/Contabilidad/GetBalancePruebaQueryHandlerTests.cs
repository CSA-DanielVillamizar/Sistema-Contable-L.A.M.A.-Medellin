using FluentAssertions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Features.Contabilidad.Queries.GetBalancePrueba;
using Moq;
using Xunit;

namespace LAMAMedellin.Application.Tests.Features.Contabilidad;

public sealed class GetBalancePruebaQueryHandlerTests
{
    private readonly Mock<ILibrosContablesRepository> _librosRepositoryMock = new();

    [Fact]
    public async Task CuandoDebeIgualaHaber_DebeReportarseCuadrado()
    {
        ConfigurarCuentas(
            Cuenta("110505", debe: 500_000m, haber: 0m),
            Cuenta("413505", debe: 0m, haber: 500_000m));

        var resultado = await Ejecutar();

        resultado.TotalDebe.Should().Be(500_000m);
        resultado.TotalHaber.Should().Be(500_000m);
        resultado.EstaCuadrado.Should().BeTrue();
    }

    [Fact]
    public async Task CuandoDebeNoIgualaHaber_DebeReportarseDescuadrado()
    {
        // El balance de prueba existe justamente para detectar esto: si no
        // cuadra hay una inconsistencia en el libro que hay que investigar
        // antes de cerrar el mes.
        ConfigurarCuentas(
            Cuenta("110505", debe: 500_000m, haber: 0m),
            Cuenta("413505", debe: 0m, haber: 400_000m));

        var resultado = await Ejecutar();

        resultado.EstaCuadrado.Should().BeFalse();
    }

    [Fact]
    public async Task SinMovimientos_DebeCuadrarEnCero()
    {
        ConfigurarCuentas();

        var resultado = await Ejecutar();

        resultado.TotalDebe.Should().Be(0m);
        resultado.TotalHaber.Should().Be(0m);
        resultado.EstaCuadrado.Should().BeTrue();
        resultado.Cuentas.Should().BeEmpty();
    }

    [Fact]
    public async Task DebePropagarElPeriodoConsultado()
    {
        ConfigurarCuentas();

        var resultado = await Ejecutar(anio: 2026, mes: 3);

        resultado.Anio.Should().Be(2026);
        resultado.Mes.Should().Be(3);
    }

    private static SaldoCuentaBalanceDto Cuenta(string codigo, decimal debe, decimal haber) =>
        new(Guid.NewGuid(), codigo, $"Cuenta {codigo}", "Debito", 0m, debe, haber, debe - haber);

    private void ConfigurarCuentas(params SaldoCuentaBalanceDto[] cuentas) =>
        _librosRepositoryMock
            .Setup(r => r.GetBalancePruebaAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuentas);

    private Task<BalancePruebaDto> Ejecutar(int anio = 2026, int mes = 7) =>
        new GetBalancePruebaQueryHandler(_librosRepositoryMock.Object)
            .Handle(new GetBalancePruebaQuery(anio, mes), CancellationToken.None);
}
