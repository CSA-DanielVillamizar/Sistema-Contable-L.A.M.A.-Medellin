using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

public sealed class CuentaPorCobrar : BaseEntity
{
    public Guid MiembroId { get; private set; }
    public Guid ConceptoCobroId { get; private set; }
    public DateOnly FechaEmision { get; private set; }
    public DateOnly FechaVencimiento { get; private set; }
    public decimal ValorTotal { get; private set; }
    public decimal SaldoPendiente { get; private set; }
    public EstadoCuentaPorCobrar Estado { get; private set; }

    public Miembro? Miembro { get; private set; }
    public ConceptoCobro? ConceptoCobro { get; private set; }

    // Constructor privado para EF Core
#pragma warning disable CS8618
    private CuentaPorCobrar() { }
#pragma warning restore CS8618

    public CuentaPorCobrar(
        Guid miembroId,
        Guid conceptoCobroId,
        DateOnly fechaEmision,
        DateOnly fechaVencimiento,
        decimal valorTotal)
    {
        if (miembroId == Guid.Empty)
        {
            throw new ArgumentException("MiembroId es obligatorio.", nameof(miembroId));
        }

        if (conceptoCobroId == Guid.Empty)
        {
            throw new ArgumentException("ConceptoCobroId es obligatorio.", nameof(conceptoCobroId));
        }

        if (fechaEmision == default)
        {
            throw new ArgumentException("FechaEmision es obligatoria.", nameof(fechaEmision));
        }

        if (fechaVencimiento == default)
        {
            throw new ArgumentException("FechaVencimiento es obligatoria.", nameof(fechaVencimiento));
        }

        if (fechaVencimiento < fechaEmision)
        {
            throw new ArgumentException("FechaVencimiento no puede ser anterior a FechaEmision.", nameof(fechaVencimiento));
        }

        if (valorTotal <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valorTotal), "ValorTotal debe ser mayor a cero.");
        }

        MiembroId = miembroId;
        ConceptoCobroId = conceptoCobroId;
        FechaEmision = fechaEmision;
        FechaVencimiento = fechaVencimiento;
        ValorTotal = valorTotal;
        SaldoPendiente = valorTotal;
        Estado = EstadoCuentaPorCobrar.Pendiente;
    }

    public void AplicarPago(decimal monto)
    {
        // Estas tres son reglas de negocio, no fallas del servidor: se lanzan como
        // ReglaNegocioException para que la API responda 400 con el mensaje real.
        // Con InvalidOperationException/ArgumentOutOfRangeException el
        // GlobalExceptionHandler las tomaba como error inesperado y devolvia 500.
        if (Estado == EstadoCuentaPorCobrar.Anulada)
        {
            throw new ReglaNegocioException("No se pueden aplicar pagos a una cuenta anulada.");
        }

        if (monto <= 0)
        {
            throw new ReglaNegocioException("El monto debe ser mayor a cero.");
        }

        if (monto > SaldoPendiente)
        {
            throw new ReglaNegocioException("El pago no puede ser mayor al saldo pendiente.");
        }

        SaldoPendiente -= monto;

        if (SaldoPendiente == 0)
        {
            Estado = EstadoCuentaPorCobrar.Pagada;
            return;
        }

        Estado = EstadoCuentaPorCobrar.PagadaParcial;
    }

    private static bool EsPeriodoValido(string? periodo)
    {
        if (string.IsNullOrWhiteSpace(periodo) || periodo.Length != 7)
        {
            return false;
        }

        if (periodo[4] != '-')
        {
            return false;
        }

        if (!int.TryParse(periodo[..4], out var anio))
        {
            return false;
        }

        if (!int.TryParse(periodo[5..], out var mes))
        {
            return false;
        }

        return anio >= 1900 && mes is >= 1 and <= 12;
    }

    private static DateOnly ParsearFechaPeriodoInicio(string periodo)
    {
        var anio = int.Parse(periodo[..4]);
        var mes = int.Parse(periodo[5..]);
        return new DateOnly(anio, mes, 1);
    }

    private static DateOnly ParsearFechaPeriodoFin(string periodo)
    {
        var inicio = ParsearFechaPeriodoInicio(periodo);
        var fin = inicio.AddMonths(1).AddDays(-1);
        return fin;
    }
}
