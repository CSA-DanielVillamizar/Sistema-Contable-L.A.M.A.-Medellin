using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.ValueObjects;

public sealed record TransaccionMultimoneda(
    string MonedaOrigen,
    decimal MontoMonedaOrigen,
    decimal TasaCambioUsada,
    DateTime FechaTasaCambio,
    FuenteTasaCambio Fuente)
{
    public string MonedaOrigen { get; init; } = ValidarMoneda(MonedaOrigen);
    public decimal MontoMonedaOrigen { get; init; } = ValidarMonto(MontoMonedaOrigen);
    public decimal TasaCambioUsada { get; init; } = ValidarTasa(TasaCambioUsada);
    public DateTime FechaTasaCambio { get; init; } = FechaTasaCambio;
    public FuenteTasaCambio Fuente { get; init; } = Fuente;

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
