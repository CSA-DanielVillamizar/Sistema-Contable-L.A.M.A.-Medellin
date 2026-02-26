using LAMAMedellin.Domain.Common;

namespace LAMAMedellin.Domain.Entities;

public sealed class AsientoContable : BaseEntity
{
    public Guid ComprobanteId { get; private set; }
    public Guid CuentaContableId { get; private set; }
    public Guid? TerceroId { get; private set; }
    public Guid CentroCostoId { get; private set; }
    public decimal Debe { get; private set; }
    public decimal Haber { get; private set; }
    public string Referencia { get; private set; }

    public Comprobante? Comprobante { get; private set; }
    public CuentaContable? CuentaContable { get; private set; }
    public CentroCosto? CentroCosto { get; private set; }

#pragma warning disable CS8618
    private AsientoContable() { }
#pragma warning restore CS8618

    public static AsientoContable Crear(
        Guid comprobanteId,
        Guid cuentaContableId,
        Guid? terceroId,
        Guid centroCostoId,
        decimal debe,
        decimal haber,
        string referencia)
    {
        ValidarReglaDebeHaber(debe, haber);

        return new AsientoContable(
            comprobanteId,
            cuentaContableId,
            terceroId,
            centroCostoId,
            debe,
            haber,
            referencia);
    }

    public AsientoContable(
        Guid comprobanteId,
        Guid cuentaContableId,
        Guid? terceroId,
        Guid centroCostoId,
        decimal debe,
        decimal haber,
        string referencia)
    {
        if (comprobanteId == Guid.Empty)
        {
            throw new ArgumentException("ComprobanteId es obligatorio.", nameof(comprobanteId));
        }

        if (cuentaContableId == Guid.Empty)
        {
            throw new ArgumentException("CuentaContableId es obligatorio.", nameof(cuentaContableId));
        }

        if (centroCostoId == Guid.Empty)
        {
            throw new ArgumentException("CentroCostoId es obligatorio.", nameof(centroCostoId));
        }

        if (debe < 0 || haber < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(debe), "Debe y Haber no pueden ser negativos.");
        }

        ValidarReglaDebeHaber(debe, haber);

        if (string.IsNullOrWhiteSpace(referencia))
        {
            throw new ArgumentException("Referencia es obligatoria.", nameof(referencia));
        }

        ComprobanteId = comprobanteId;
        CuentaContableId = cuentaContableId;
        TerceroId = terceroId;
        CentroCostoId = centroCostoId;
        Debe = debe;
        Haber = haber;
        Referencia = referencia.Trim();
    }

    private static void ValidarReglaDebeHaber(decimal debe, decimal haber)
    {
        if (debe > 0 && haber > 0)
        {
            throw new ReglaNegocioException("Un asiento no puede tener Debe y Haber mayores a cero al mismo tiempo.");
        }

        if (debe == 0 && haber == 0)
        {
            throw new ReglaNegocioException("Un asiento debe tener valor en Debe o Haber.");
        }
    }
}
