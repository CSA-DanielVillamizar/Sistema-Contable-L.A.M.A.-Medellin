using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.ValueObjects;

/// <summary>
/// Datos de una transaccion pactada en moneda extranjera (historia fx-01).
///
/// El soporte es la referencia al documento que respalda la tasa aplicada: el
/// extracto, la certificacion de la SFC o el comprobante de la plataforma. Sin
/// el, la tasa usada es la palabra de quien registro, y ante una revision no
/// hay como sustentarla.
/// </summary>
public sealed record TransaccionMultimoneda(
    string MonedaOrigen,
    decimal MontoMonedaOrigen,
    decimal TasaCambioUsada,
    DateTime FechaTasaCambio,
    FuenteTasaCambio Fuente,
    string? ReferenciaSoporte = null)
{
    public string MonedaOrigen { get; init; } = ValidarMoneda(MonedaOrigen);
    public decimal MontoMonedaOrigen { get; init; } = ValidarMonto(MontoMonedaOrigen);
    public decimal TasaCambioUsada { get; init; } = ValidarTasa(TasaCambioUsada);
    public DateTime FechaTasaCambio { get; init; } = FechaTasaCambio;
    public FuenteTasaCambio Fuente { get; init; } = Fuente;

    /// <summary>
    /// Obligatorio salvo que la tasa sea la TRM oficial de la SFC, que es
    /// publica y verificable por fecha sin necesidad de adjunto.
    /// </summary>
    public string? ReferenciaSoporte { get; init; } = ValidarSoporte(ReferenciaSoporte, Fuente);

    private static string? ValidarSoporte(string? referenciaSoporte, FuenteTasaCambio fuente)
    {
        var soporte = string.IsNullOrWhiteSpace(referenciaSoporte) ? null : referenciaSoporte.Trim();

        // La TRM de la SFC es publica y verificable por fecha, asi que no
        // necesita adjunto. La tasa del banco y la digitada a mano si: sin el
        // soporte, la tasa aplicada es la palabra de quien registro.
        if (fuente != FuenteTasaCambio.TrmSfc && soporte is null)
        {
            throw new ArgumentException(
                "Una tasa que no proviene de la TRM oficial exige indicar su soporte.",
                nameof(referenciaSoporte));
        }

        return soporte;
    }

    private static string ValidarMoneda(string monedaOrigen)
    {
        if (string.IsNullOrWhiteSpace(monedaOrigen))
        {
            throw new ArgumentException("MonedaOrigen es obligatoria.", nameof(monedaOrigen));
        }

        return monedaOrigen.Trim().ToUpperInvariant();
    }

    private static decimal ValidarMonto(decimal montoMonedaOrigen)
    {
        if (montoMonedaOrigen <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(montoMonedaOrigen), "MontoMonedaOrigen debe ser mayor a cero.");
        }

        return montoMonedaOrigen;
    }

    private static decimal ValidarTasa(decimal tasaCambioUsada)
    {
        if (tasaCambioUsada <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tasaCambioUsada), "TasaCambioUsada debe ser mayor a cero.");
        }

        return tasaCambioUsada;
    }
}
