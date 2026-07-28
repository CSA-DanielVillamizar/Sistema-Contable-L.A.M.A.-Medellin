using FluentAssertions;
using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Features.Contabilidad.Queries.GetLibroMayor;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Moq;
using Xunit;

namespace LAMAMedellin.Application.Tests.Features.Contabilidad;

public sealed class GetLibroMayorQueryHandlerTests
{
    private static readonly DateOnly Desde = new(2026, 7, 1);
    private static readonly DateOnly Hasta = new(2026, 7, 31);

    private readonly Mock<ILibrosContablesRepository> _librosRepositoryMock = new();

    [Fact]
    public async Task CuandoLaCuentaNoExiste_DebeLanzarExcepcionNegocio()
    {
        _librosRepositoryMock
            .Setup(r => r.GetCuentaAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CuentaContable?)null);

        var act = async () => await Ejecutar(Guid.NewGuid());

        await act.Should().ThrowAsync<ExcepcionNegocio>().WithMessage("*no existe*");
    }

    [Fact]
    public async Task CuentaDebito_DebeAcumularSaldoSumandoElDebe()
    {
        // Una cuenta de activo o gasto crece con el debe.
        var cuenta = CrearCuenta("110505", "Caja general", NaturalezaCuenta.Debito);
        ConfigurarCuenta(cuenta);
        ConfigurarAcumuladoAnterior(debe: 500_000m, haber: 100_000m);
        ConfigurarMovimientos(
            Movimiento(debe: 200_000m, haber: 0m),
            Movimiento(debe: 0m, haber: 50_000m));

        var resultado = await Ejecutar(cuenta.Id);

        resultado.SaldoAnterior.Should().Be(400_000m);
        resultado.Movimientos[0].SaldoAcumulado.Should().Be(600_000m);
        resultado.Movimientos[1].SaldoAcumulado.Should().Be(550_000m);
        resultado.SaldoFinal.Should().Be(550_000m);
    }

    [Fact]
    public async Task CuentaCredito_DebeAcumularSaldoSumandoElHaber()
    {
        // Una cuenta de ingreso crece con el haber. Restar siempre en el mismo
        // sentido mostraria los ingresos en negativo.
        var cuenta = CrearCuenta("413505", "Ingresos por cuotas", NaturalezaCuenta.Credito);
        ConfigurarCuenta(cuenta);
        ConfigurarAcumuladoAnterior(debe: 0m, haber: 1_000_000m);
        ConfigurarMovimientos(
            Movimiento(debe: 0m, haber: 300_000m),
            Movimiento(debe: 100_000m, haber: 0m));

        var resultado = await Ejecutar(cuenta.Id);

        resultado.SaldoAnterior.Should().Be(1_000_000m);
        resultado.Movimientos[0].SaldoAcumulado.Should().Be(1_300_000m);
        resultado.Movimientos[1].SaldoAcumulado.Should().Be(1_200_000m);
        resultado.SaldoFinal.Should().Be(1_200_000m);
    }

    [Fact]
    public async Task SinMovimientos_ElSaldoFinalDebeIgualarAlAnterior()
    {
        var cuenta = CrearCuenta("110505", "Caja general", NaturalezaCuenta.Debito);
        ConfigurarCuenta(cuenta);
        ConfigurarAcumuladoAnterior(debe: 750_000m, haber: 0m);
        ConfigurarMovimientos();

        var resultado = await Ejecutar(cuenta.Id);

        resultado.SaldoAnterior.Should().Be(750_000m);
        resultado.SaldoFinal.Should().Be(750_000m);
        resultado.Movimientos.Should().BeEmpty();
    }

    [Fact]
    public async Task DebeTotalizarDebeYHaberDelRango()
    {
        var cuenta = CrearCuenta("110505", "Caja general", NaturalezaCuenta.Debito);
        ConfigurarCuenta(cuenta);
        ConfigurarAcumuladoAnterior(debe: 0m, haber: 0m);
        ConfigurarMovimientos(
            Movimiento(debe: 100_000m, haber: 0m),
            Movimiento(debe: 50_000m, haber: 0m),
            Movimiento(debe: 0m, haber: 30_000m));

        var resultado = await Ejecutar(cuenta.Id);

        resultado.TotalDebe.Should().Be(150_000m);
        resultado.TotalHaber.Should().Be(30_000m);
    }

    private static CuentaContable CrearCuenta(string codigo, string descripcion, NaturalezaCuenta naturaleza) =>
        new(codigo, descripcion, naturaleza, permiteMovimiento: true);

    private void ConfigurarCuenta(CuentaContable cuenta) =>
        _librosRepositoryMock
            .Setup(r => r.GetCuentaAsync(cuenta.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cuenta);

    private void ConfigurarAcumuladoAnterior(decimal debe, decimal haber) =>
        _librosRepositoryMock
            .Setup(r => r.GetAcumuladoAnteriorAsync(
                It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((debe, haber));

    private void ConfigurarMovimientos(params MovimientoLibroMayorDto[] movimientos) =>
        _librosRepositoryMock
            .Setup(r => r.GetMovimientosMayorAsync(
                It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(movimientos);

    private static MovimientoLibroMayorDto Movimiento(decimal debe, decimal haber) =>
        new(
            new DateTime(2026, 7, 10, 0, 0, 0, DateTimeKind.Utc),
            "ING-00000001",
            "Movimiento de prueba",
            "Capitulo",
            "Referencia",
            debe,
            haber,
            0m);

    private Task<LibroMayorDto> Ejecutar(Guid cuentaId) =>
        new GetLibroMayorQueryHandler(_librosRepositoryMock.Object)
            .Handle(new GetLibroMayorQuery(cuentaId, Desde, Hasta), CancellationToken.None);
}
