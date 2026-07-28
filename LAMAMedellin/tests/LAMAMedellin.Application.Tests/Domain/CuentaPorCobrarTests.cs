using FluentAssertions;
using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Xunit;

namespace LAMAMedellin.Application.Tests.Domain;

/// <summary>
/// Las violaciones de regla de negocio deben viajar como ReglaNegocioException.
/// GlobalExceptionHandler la mapea a 400 con el mensaje real; cualquier otro tipo
/// cae en el caso por defecto y se convierte en un 500 "Ocurrio un error inesperado",
/// que es lo que pasaba antes con InvalidOperationException.
/// </summary>
public sealed class CuentaPorCobrarTests
{
    [Fact]
    public void AplicarPago_CuandoElMontoSuperaElSaldo_DebeLanzarReglaNegocio()
    {
        var cuenta = CrearCuentaPorCobrar(100_000m);

        var act = () => cuenta.AplicarPago(150_000m);

        act.Should().Throw<ReglaNegocioException>()
            .WithMessage("El pago no puede ser mayor al saldo pendiente.");
    }

    [Fact]
    public void AplicarPago_CuandoElMontoNoEsPositivo_DebeLanzarReglaNegocio()
    {
        var cuenta = CrearCuentaPorCobrar(100_000m);

        var act = () => cuenta.AplicarPago(0m);

        act.Should().Throw<ReglaNegocioException>()
            .WithMessage("El monto debe ser mayor a cero.");
    }

    [Fact]
    public void AplicarPago_SobreCuentaSaldada_DebeLanzarReglaNegocio()
    {
        var cuenta = CrearCuentaPorCobrar(100_000m);
        cuenta.AplicarPago(100_000m);
        cuenta.Estado.Should().Be(EstadoCuentaPorCobrar.Pagada);

        var act = () => cuenta.AplicarPago(1m);

        act.Should().Throw<ReglaNegocioException>();
    }

    [Fact]
    public void AplicarPago_Parcial_DebeDejarSaldoYEstadoPagadaParcial()
    {
        var cuenta = CrearCuentaPorCobrar(100_000m);

        cuenta.AplicarPago(30_000m);

        cuenta.SaldoPendiente.Should().Be(70_000m);
        cuenta.Estado.Should().Be(EstadoCuentaPorCobrar.PagadaParcial);
    }

    [Fact]
    public void AplicarPago_QueSaldaElTotal_DebeDejarEstadoPagada()
    {
        var cuenta = CrearCuentaPorCobrar(100_000m);

        cuenta.AplicarPago(60_000m);
        cuenta.AplicarPago(40_000m);

        cuenta.SaldoPendiente.Should().Be(0m);
        cuenta.Estado.Should().Be(EstadoCuentaPorCobrar.Pagada);
    }

    private static CuentaPorCobrar CrearCuentaPorCobrar(decimal valorTotal)
    {
        var fechaEmision = new DateOnly(2026, 2, 1);

        return new CuentaPorCobrar(
            miembroId: Guid.NewGuid(),
            conceptoCobroId: Guid.NewGuid(),
            fechaEmision: fechaEmision,
            fechaVencimiento: fechaEmision.AddMonths(1).AddDays(-1),
            valorTotal: valorTotal);
    }
}
