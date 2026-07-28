using LAMAMedellin.Domain.Common;

namespace LAMAMedellin.Domain.Entities;

public sealed class CuotaAsamblea : BaseEntity
{
    public int Anio { get; private set; }
    public decimal ValorMensualCOP { get; private set; }
    public int MesInicioCobro { get; private set; }
    public string? ActaSoporte { get; private set; }

    /// <summary>
    /// Valor en USD de la cuota de renovación anual de membresía internacional L.A.M.A.,
    /// cobrada en diciembre de cada año. Nulo si no aplica para el año dado.
    /// El valor en COP se calcula usando la TRM vigente en el momento del cobro (diciembre).
    /// </summary>
    public decimal? RenovacionMembresiaUSD { get; private set; }

    // Constructor privado para EF Core
#pragma warning disable CS8618
    private CuotaAsamblea() { }
#pragma warning restore CS8618

    public CuotaAsamblea(int anio, decimal valorMensualCop, int mesInicioCobro, string? actaSoporte = null, decimal? renovacionMembresiaUsd = null)
    {
        if (anio <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(anio), "Anio debe ser mayor a cero.");
        }

        if (valorMensualCop <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valorMensualCop), "ValorMensualCOP debe ser mayor a cero.");
        }

        if (mesInicioCobro < 1 || mesInicioCobro > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(mesInicioCobro), "MesInicioCobro debe estar entre 1 y 12.");
        }

        if (renovacionMembresiaUsd.HasValue && renovacionMembresiaUsd.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(renovacionMembresiaUsd), "RenovacionMembresiaUSD debe ser mayor a cero.");
        }

        Anio = anio;
        ValorMensualCOP = valorMensualCop;
        MesInicioCobro = mesInicioCobro;
        ActaSoporte = string.IsNullOrWhiteSpace(actaSoporte) ? null : actaSoporte.Trim();
        RenovacionMembresiaUSD = renovacionMembresiaUsd;
    }

    /// <summary>
    /// Actualiza el valor de la cuota de renovación de membresía internacional para este año.
    /// </summary>
    public void ActualizarRenovacionMembresiaUSD(decimal? valorUsd)
    {
        if (valorUsd.HasValue && valorUsd.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valorUsd), "RenovacionMembresiaUSD debe ser mayor a cero.");
        }

        RenovacionMembresiaUSD = valorUsd;
    }
}
