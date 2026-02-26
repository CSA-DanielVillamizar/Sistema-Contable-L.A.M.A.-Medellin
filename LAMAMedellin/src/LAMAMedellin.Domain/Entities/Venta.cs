using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

public sealed class Venta : BaseEntity
{
    public string NumeroFacturaInterna { get; private set; }
    public DateTime Fecha { get; private set; }
    public Guid? CompradorId { get; private set; }
    public decimal Total { get; private set; }
    public MetodoPagoVenta MetodoPago { get; private set; }
    public List<DetalleVenta> DetallesVenta { get; private set; } = [];

#pragma warning disable CS8618
    private Venta() { }
#pragma warning restore CS8618

    public Venta(
        string numeroFacturaInterna,
        DateTime fecha,
        Guid? compradorId,
        decimal total,
        MetodoPagoVenta metodoPago)
    {
        if (string.IsNullOrWhiteSpace(numeroFacturaInterna))
        {
            throw new ArgumentException("NumeroFacturaInterna es obligatorio.", nameof(numeroFacturaInterna));
        }

        if (total < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(total), "Total no puede ser negativo.");
        }

        NumeroFacturaInterna = numeroFacturaInterna.Trim();
        Fecha = fecha;
        CompradorId = compradorId;
        Total = total;
        MetodoPago = metodoPago;
    }

    public void AgregarDetalle(DetalleVenta detalle)
    {
        if (detalle is null)
        {
            throw new ArgumentNullException(nameof(detalle));
        }

        DetallesVenta.Add(detalle);
    }

    public void AsignarTotal(decimal total)
    {
        if (total < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(total), "Total no puede ser negativo.");
        }

        Total = total;
    }
}
