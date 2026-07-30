using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

/// <summary>
/// Obligacion con un proveedor: una factura recibida que aun no se ha pagado
/// (historias 1-13 y 1-14).
///
/// Es el reflejo de CuentaPorCobrar. Sin esta entidad, una factura pendiente no
/// existia en ninguna parte hasta que se pagaba, de modo que el pasivo del
/// capitulo quedaba fuera del balance y nadie podia saber cuanto se debia.
///
/// El proveedor se guarda como texto y no como entidad propia: el capitulo
/// trabaja con pocos proveedores y ocasionales, y crear un maestro completo
/// obligaria a darlos de alta antes de poder registrar una factura. El NIT
/// queda aparte porque es lo que exige la exogena.
/// </summary>
public sealed class CuentaPorPagar : BaseEntity
{
    public string NombreProveedor { get; private set; }

    /// <summary>Documento del proveedor. Lo exige la exogena de la DIAN.</summary>
    public string NitProveedor { get; private set; }

    /// <summary>Numero de la factura del proveedor, tal como el la emitio.</summary>
    public string NumeroFactura { get; private set; }

    public string Concepto { get; private set; }

    /// <summary>
    /// Cuenta de gasto o costo contra la que se reconoce la obligacion. La
    /// contrapartida es siempre una cuenta por pagar del pasivo.
    /// </summary>
    public Guid CuentaContableGastoId { get; private set; }

    public Guid CentroCostoId { get; private set; }

    public DateOnly FechaEmision { get; private set; }
    public DateOnly FechaVencimiento { get; private set; }

    public decimal ValorTotal { get; private set; }
    public decimal SaldoPendiente { get; private set; }
    public EstadoCuentaPorPagar Estado { get; private set; }

    public CuentaContable? CuentaContableGasto { get; private set; }
    public CentroCosto? CentroCosto { get; private set; }

#pragma warning disable CS8618
    private CuentaPorPagar() { }
#pragma warning restore CS8618

    public CuentaPorPagar(
        string nombreProveedor,
        string nitProveedor,
        string numeroFactura,
        string concepto,
        Guid cuentaContableGastoId,
        Guid centroCostoId,
        DateOnly fechaEmision,
        DateOnly fechaVencimiento,
        decimal valorTotal)
    {
        if (string.IsNullOrWhiteSpace(nombreProveedor))
        {
            throw new ReglaNegocioException("NombreProveedor es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(nitProveedor))
        {
            throw new ReglaNegocioException("NitProveedor es obligatorio: sin el la factura no puede reportarse en la exogena.");
        }

        if (string.IsNullOrWhiteSpace(numeroFactura))
        {
            throw new ReglaNegocioException("NumeroFactura es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(concepto))
        {
            throw new ReglaNegocioException("Concepto es obligatorio.");
        }

        if (cuentaContableGastoId == Guid.Empty)
        {
            throw new ReglaNegocioException("CuentaContableGastoId es obligatorio.");
        }

        if (centroCostoId == Guid.Empty)
        {
            throw new ReglaNegocioException("CentroCostoId es obligatorio.");
        }

        if (fechaEmision == default)
        {
            throw new ReglaNegocioException("FechaEmision es obligatoria.");
        }

        if (fechaVencimiento == default)
        {
            throw new ReglaNegocioException("FechaVencimiento es obligatoria.");
        }

        if (fechaVencimiento < fechaEmision)
        {
            throw new ReglaNegocioException("FechaVencimiento no puede ser anterior a FechaEmision.");
        }

        if (valorTotal <= 0)
        {
            throw new ReglaNegocioException("ValorTotal debe ser mayor a cero.");
        }

        NombreProveedor = nombreProveedor.Trim();
        NitProveedor = nitProveedor.Trim();
        NumeroFactura = numeroFactura.Trim();
        Concepto = concepto.Trim();
        CuentaContableGastoId = cuentaContableGastoId;
        CentroCostoId = centroCostoId;
        FechaEmision = fechaEmision;
        FechaVencimiento = fechaVencimiento;
        ValorTotal = valorTotal;
        SaldoPendiente = valorTotal;
        Estado = EstadoCuentaPorPagar.Pendiente;
    }

    /// <summary>
    /// Cruza un pago contra la obligacion. Igual que en cartera, un abono
    /// parcial la deja pendiente: darla por saldada al primer pago fue el bug
    /// que se corrigio en el lado de cobrar.
    /// </summary>
    public void AplicarPago(decimal monto)
    {
        if (Estado == EstadoCuentaPorPagar.Anulada)
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
        Estado = SaldoPendiente == 0 ? EstadoCuentaPorPagar.Pagada : EstadoCuentaPorPagar.PagadaParcial;
    }

    /// <summary>
    /// Anula la obligacion. Solo si no se ha pagado nada: una factura con
    /// abonos ya movio dinero, y anularla dejaria esos pagos sin explicacion.
    /// </summary>
    public void Anular()
    {
        if (Estado != EstadoCuentaPorPagar.Pendiente)
        {
            throw new ReglaNegocioException(
                "Solo se puede anular una cuenta por pagar sin pagos aplicados.");
        }

        Estado = EstadoCuentaPorPagar.Anulada;
    }
}
