using FluentAssertions;
using LAMAMedellin.Domain.Enums;
using LAMAMedellin.Domain.ValueObjects;
using Xunit;

namespace LAMAMedellin.Application.Tests.Domain;

/// <summary>
/// Soporte de la tasa de cambio (historia fx-01).
///
/// El criterio pide guardar USD, tasa y soporte. Sin el soporte, la tasa
/// aplicada es la palabra de quien registro y ante una revision no hay como
/// sustentarla.
/// </summary>
public sealed class TransaccionMultimonedaTests
{
    private static TransaccionMultimoneda Crear(FuenteTasaCambio fuente, string? soporte)
    {
        return new TransaccionMultimoneda(
            MonedaOrigen: "usd",
            MontoMonedaOrigen: 20m,
            TasaCambioUsada: 4100m,
            FechaTasaCambio: DateTime.UtcNow,
            Fuente: fuente,
            ReferenciaSoporte: soporte);
    }

    [Fact]
    public void La_TRM_oficial_no_exige_soporte()
    {
        // Es publica y verificable por fecha; pedir adjunto seria trabajo
        // inutil para el tesorero.
        var accion = () => Crear(FuenteTasaCambio.TrmSfc, null);

        accion.Should().NotThrow();
    }

    [Theory]
    [InlineData(FuenteTasaCambio.TasaBanco)]
    [InlineData(FuenteTasaCambio.ManualConSoporte)]
    public void Una_tasa_que_no_es_la_oficial_exige_soporte(FuenteTasaCambio fuente)
    {
        var accion = () => Crear(fuente, null);

        accion.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Con_soporte_indicado_la_tasa_del_banco_se_acepta()
    {
        var multimoneda = Crear(FuenteTasaCambio.TasaBanco, "  Extracto Bancolombia 2026-12-20  ");

        multimoneda.ReferenciaSoporte.Should().Be("Extracto Bancolombia 2026-12-20");
    }

    [Fact]
    public void La_moneda_se_normaliza_en_mayusculas()
    {
        var multimoneda = Crear(FuenteTasaCambio.TrmSfc, null);

        multimoneda.MonedaOrigen.Should().Be("USD");
    }

    [Fact]
    public void Un_soporte_en_blanco_cuenta_como_ausente()
    {
        var accion = () => Crear(FuenteTasaCambio.ManualConSoporte, "   ");

        // Aceptar espacios dejaria pasar el control sin aportar nada.
        accion.Should().Throw<ArgumentException>();
    }
}
