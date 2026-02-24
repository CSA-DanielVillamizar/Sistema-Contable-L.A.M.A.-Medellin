using System.Text.RegularExpressions;
using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

public sealed partial class CuentaPorCobrar : BaseEntity
{
    public Guid MiembroId { get; private set; }
    public string Periodo { get; private set; }
    public decimal ValorEsperadoCOP { get; private set; }
    public decimal SaldoPendienteCOP { get; private set; }
    public EstadoCuentaPorCobrar Estado { get; private set; }

    public Miembro? Miembro { get; private set; }

    // Constructor privado para EF Core
#pragma warning disable CS8618
    private CuentaPorCobrar() { }
#pragma warning restore CS8618

    public CuentaPorCobrar(Guid miembroId, string periodo, decimal valorEsperadoCop)
    {
        if (miembroId == Guid.Empty)
        {
            throw new ArgumentException("MiembroId es obligatorio.", nameof(miembroId));
        }

        if (string.IsNullOrWhiteSpace(periodo) || !PeriodoRegex().IsMatch(periodo))
        {
            throw new ArgumentException("Periodo debe tener formato YYYY-MM.", nameof(periodo));
        }

        if (valorEsperadoCop <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valorEsperadoCop), "ValorEsperadoCOP debe ser mayor a cero.");
        }

        MiembroId = miembroId;
        Periodo = periodo.Trim();
        ValorEsperadoCOP = valorEsperadoCop;
        SaldoPendienteCOP = valorEsperadoCop;
        Estado = EstadoCuentaPorCobrar.Pendiente;
    }

    public void AplicarAbono(decimal monto)
    {
        if (monto <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(monto), "El monto debe ser mayor a cero.");
        }

        if (monto > SaldoPendienteCOP)
        {
            throw new InvalidOperationException("El abono no puede ser mayor al saldo pendiente.");
        }

        SaldoPendienteCOP -= monto;

        if (SaldoPendienteCOP == 0)
        {
            Estado = EstadoCuentaPorCobrar.Pagado;
        }
    }

    [GeneratedRegex(@"^\d{4}-(0[1-9]|1[0-2])$")]
    private static partial Regex PeriodoRegex();
}
