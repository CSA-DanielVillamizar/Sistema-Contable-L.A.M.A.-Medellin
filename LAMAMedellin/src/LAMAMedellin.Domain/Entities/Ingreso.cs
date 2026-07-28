using LAMAMedellin.Domain.Common;

namespace LAMAMedellin.Domain.Entities;

public sealed class Ingreso : BaseEntity
{
    public DateTime Fecha { get; private set; }
    public decimal Monto { get; private set; }
    public string Concepto { get; private set; } = string.Empty;
    public Guid? TerceroId { get; private set; }
    public Guid CuentaContableId { get; private set; }
    public Guid BancoId { get; private set; }
    public Guid CentroCostoId { get; private set; }
    public Guid? ComprobanteContableId { get; private set; }

    private Ingreso() { }

    public Ingreso(
        DateTime fecha,
        decimal monto,
        string concepto,
        Guid? terceroId,
        Guid cuentaContableId,
        Guid bancoId,
        Guid centroCostoId)
    {
        if (monto <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(monto), "Monto debe ser mayor a cero.");
        }

        if (string.IsNullOrWhiteSpace(concepto))
        {
            throw new ArgumentException("Concepto es obligatorio.", nameof(concepto));
        }

        if (cuentaContableId == Guid.Empty)
        {
            throw new ArgumentException("CuentaContableId es obligatorio.", nameof(cuentaContableId));
        }

        if (bancoId == Guid.Empty)
        {
            throw new ArgumentException("BancoId es obligatorio.", nameof(bancoId));
        }

        if (centroCostoId == Guid.Empty)
        {
            throw new ArgumentException("CentroCostoId es obligatorio.", nameof(centroCostoId));
        }

        Fecha = fecha;
        Monto = monto;
        Concepto = concepto.Trim();
        TerceroId = terceroId;
        CuentaContableId = cuentaContableId;
        BancoId = bancoId;
        CentroCostoId = centroCostoId;
    }

    public void AsignarComprobanteContable(Guid comprobanteId)
    {
        if (comprobanteId == Guid.Empty)
        {
            throw new ArgumentException("ComprobanteContableId es obligatorio.", nameof(comprobanteId));
        }

        ComprobanteContableId = comprobanteId;
    }
}
