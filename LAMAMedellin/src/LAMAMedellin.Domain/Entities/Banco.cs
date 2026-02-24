using LAMAMedellin.Domain.Common;

namespace LAMAMedellin.Domain.Entities;

public sealed class Banco : BaseEntity
{
    public string NumeroCuenta { get; private set; }
    public decimal SaldoActual { get; private set; }

    public Banco(string numeroCuenta, decimal saldoActual)
    {
        if (string.IsNullOrWhiteSpace(numeroCuenta))
        {
            throw new ArgumentException("NumeroCuenta es obligatorio.", nameof(numeroCuenta));
        }

        NumeroCuenta = numeroCuenta.Trim();
        SaldoActual = saldoActual;
    }

    public void AplicarIngreso(decimal monto)
    {
        if (monto <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(monto), "El monto debe ser mayor a cero.");
        }

        SaldoActual += monto;
    }

    public void AplicarEgreso(decimal monto)
    {
        if (monto <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(monto), "El monto debe ser mayor a cero.");
        }

        SaldoActual -= monto;
    }
}
