using FluentAssertions;
using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Entities;
using Xunit;

namespace LAMAMedellin.Application.Tests.Domain;

/// <summary>
/// Diferencia en cambio (historia 1-17). El signo es lo que decide si el
/// asiento va a ganancia o a perdida, asi que conviene fijarlo aqui y no
/// deducirlo leyendo el handler.
/// </summary>
public sealed class DiferenciaEnCambioTests
{
    private static CuentaPorPagar CrearEnUSD(decimal valorUSD, decimal tasaReconocida)
    {
        return new CuentaPorPagar(
            nombreProveedor: "Comite Internacional L.A.M.A.",
            nitProveedor: "EXT-0001",
            numeroFactura: "INT-2026-01",
            concepto: "Renovacion de membresia internacional",
            cuentaContableGastoId: Guid.NewGuid(),
            centroCostoId: Guid.NewGuid(),
            fechaEmision: DateOnly.FromDateTime(DateTime.UtcNow),
            fechaVencimiento: DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30),
            valorTotal: valorUSD * tasaReconocida,
            valorUSD: valorUSD,
            tasaCambioReconocida: tasaReconocida);
    }

    [Fact]
    public void Liquidar_a_una_tasa_mayor_produce_perdida()
    {
        var cuenta = CrearEnUSD(100m, 4000m);

        var diferencia = cuenta.CalcularDiferenciaEnCambio(montoUSDPagado: 100m, tasaLiquidacion: 4200m);

        // Se entregaron 420.000 por un pasivo reconocido en 400.000.
        diferencia.Should().Be(20000m, "positivo es perdida: salieron mas pesos de los reconocidos");
    }

    [Fact]
    public void Liquidar_a_una_tasa_menor_produce_ganancia()
    {
        var cuenta = CrearEnUSD(100m, 4000m);

        var diferencia = cuenta.CalcularDiferenciaEnCambio(montoUSDPagado: 100m, tasaLiquidacion: 3800m);

        diferencia.Should().Be(-20000m);
    }

    [Fact]
    public void Si_la_tasa_no_cambia_no_hay_diferencia()
    {
        var cuenta = CrearEnUSD(100m, 4000m);

        cuenta.CalcularDiferenciaEnCambio(100m, 4000m).Should().Be(0m);
    }

    [Fact]
    public void Un_pago_parcial_produce_la_diferencia_proporcional()
    {
        var cuenta = CrearEnUSD(100m, 4000m);

        var diferencia = cuenta.CalcularDiferenciaEnCambio(montoUSDPagado: 40m, tasaLiquidacion: 4200m);

        diferencia.Should().Be(8000m, "la diferencia corresponde solo a los dolares liquidados");
    }

    [Fact]
    public void Una_obligacion_en_pesos_no_genera_diferencia()
    {
        var cuenta = new CuentaPorPagar(
            "Proveedor local", "900123456-1", "F-001", "Servicio",
            Guid.NewGuid(), Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30),
            valorTotal: 500000m);

        cuenta.EsEnMonedaExtranjera.Should().BeFalse();
        cuenta.CalcularDiferenciaEnCambio(100m, 4200m).Should().Be(0m);
    }

    [Fact]
    public void No_se_admite_valor_en_USD_sin_tasa_de_reconocimiento()
    {
        var crear = () => new CuentaPorPagar(
            "Proveedor", "900123456-1", "F-002", "Servicio",
            Guid.NewGuid(), Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30),
            valorTotal: 400000m,
            valorUSD: 100m,
            tasaCambioReconocida: null);

        // Con uno solo de los dos no hay forma de calcular la diferencia despues.
        crear.Should().Throw<ReglaNegocioException>();
    }
}
