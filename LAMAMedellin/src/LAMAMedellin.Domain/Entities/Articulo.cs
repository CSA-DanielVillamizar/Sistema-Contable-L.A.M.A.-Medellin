using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

public sealed class Articulo : BaseEntity
{
    public string Nombre { get; private set; }
    public string SKU { get; private set; }
    public string Descripcion { get; private set; }
    public CategoriaArticulo Categoria { get; private set; }
    public decimal PrecioVenta { get; private set; }
    public decimal CostoPromedio { get; private set; }
    public int StockActual { get; private set; }
    public Guid CuentaContableIngresoId { get; private set; }

    public CuentaContable? CuentaContableIngreso { get; private set; }
    public List<DetalleVenta> DetallesVenta { get; private set; } = [];

#pragma warning disable CS8618
    private Articulo() { }
#pragma warning restore CS8618

    public Articulo(
        string nombre,
        string sku,
        string descripcion,
        CategoriaArticulo categoria,
        decimal precioVenta,
        decimal costoPromedio,
        int stockActual,
        Guid cuentaContableIngresoId)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("Nombre es obligatorio.", nameof(nombre));
        }

        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new ArgumentException("SKU es obligatorio.", nameof(sku));
        }

        if (string.IsNullOrWhiteSpace(descripcion))
        {
            throw new ArgumentException("Descripcion es obligatoria.", nameof(descripcion));
        }

        if (precioVenta <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(precioVenta), "PrecioVenta debe ser mayor a cero.");
        }

        if (costoPromedio < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(costoPromedio), "CostoPromedio no puede ser negativo.");
        }

        if (stockActual < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stockActual), "StockActual no puede ser negativo.");
        }

        if (cuentaContableIngresoId == Guid.Empty)
        {
            throw new ArgumentException("CuentaContableIngresoId es obligatorio.", nameof(cuentaContableIngresoId));
        }

        Nombre = nombre.Trim();
        SKU = sku.Trim();
        Descripcion = descripcion.Trim();
        Categoria = categoria;
        PrecioVenta = precioVenta;
        CostoPromedio = costoPromedio;
        StockActual = stockActual;
        CuentaContableIngresoId = cuentaContableIngresoId;
    }

    public void ReducirStock(int cantidad)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cantidad), "Cantidad debe ser mayor a cero.");
        }

        if (cantidad > StockActual)
        {
            throw new ReglaNegocioException("No hay stock suficiente para el artículo indicado.");
        }

        StockActual -= cantidad;
    }
}
