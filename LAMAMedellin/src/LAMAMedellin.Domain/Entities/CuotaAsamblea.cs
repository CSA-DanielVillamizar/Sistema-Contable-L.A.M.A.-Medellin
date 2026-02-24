using LAMAMedellin.Domain.Common;

namespace LAMAMedellin.Domain.Entities;

public sealed class CuotaAsamblea : BaseEntity
{
    public int Anio { get; private set; }
    public decimal ValorMensualCOP { get; private set; }
    public int MesInicioCobro { get; private set; }
    public string? ActaSoporte { get; private set; }

    // Constructor privado para EF Core
#pragma warning disable CS8618
    private CuotaAsamblea() { }
#pragma warning restore CS8618

    public CuotaAsamblea(int anio, decimal valorMensualCop, int mesInicioCobro, string? actaSoporte = null)
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

        Anio = anio;
        ValorMensualCOP = valorMensualCop;
        MesInicioCobro = mesInicioCobro;
        ActaSoporte = string.IsNullOrWhiteSpace(actaSoporte) ? null : actaSoporte.Trim();
    }
}
