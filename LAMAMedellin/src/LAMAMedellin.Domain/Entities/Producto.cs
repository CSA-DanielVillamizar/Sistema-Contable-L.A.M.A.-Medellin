using LAMAMedellin.Domain.Common;

namespace LAMAMedellin.Domain.Entities;

/// <summary>
/// Entidad que representa un producto de merchandising de la tienda del capítulo.
/// </summary>
public sealed class Producto : BaseEntity
{
    /// <summary>Nombre del producto</summary>
    public string Nombre { get; private set; }

    /// <summary>código SKU único del producto</summary>
    public string SKU { get; private set; }

    /// <summary>Precio de venta en COP</summary>
    public decimal PrecioVentaCOP { get; private set; }

    /// <summary>Cantidad disponible en stock</summary>
    public int CantidadStock { get; private set; }

    /// <summary>ID de la cuenta contable asociada a los ingresos de este producto</summary>
    public Guid CuentaContableIngresoId { get; private set; }

    /// <summary>Navegación a la cuenta contable</summary>
    public CuentaContable? CuentaContableIngreso { get; private set; }

    /// <summary>Colección de movimientos de inventario relacionados</summary>
    public List<MovimientoInventario> Movimientos { get; private set; } = [];

#pragma warning disable CS8618
    private Producto() { }
#pragma warning restore CS8618

    /// <summary>
    /// Constructor de Producto.
    /// </summary>
    /// <param name="nombre">Nombre del producto</param>
    /// <param name="sku">Código SKU del producto</param>
    /// <param name="precioVentaCOP">Precio de venta en COP</param>
    /// <param name="cantidadStock">Cantidad inicial en stock</param>
    /// <param name="cuentaContableIngresoId">ID de la cuenta contable para registrar ingresos</param>
    public Producto(
        string nombre,
        string sku,
        decimal precioVentaCOP,
        int cantidadStock,
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

        if (precioVentaCOP <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(precioVentaCOP), "PrecioVentaCOP debe ser mayor a cero.");
        }

        if (cantidadStock < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cantidadStock), "CantidadStock no puede ser negativo.");
        }

        if (cuentaContableIngresoId == Guid.Empty)
        {
            throw new ArgumentException("CuentaContableIngresoId es obligatorio.", nameof(cuentaContableIngresoId));
        }

        Nombre = nombre.Trim();
        SKU = sku.Trim().ToUpper();
        PrecioVentaCOP = precioVentaCOP;
        CantidadStock = cantidadStock;
        CuentaContableIngresoId = cuentaContableIngresoId;
    }

    /// <summary>
    /// Ajusta el stock del producto.
    /// </summary>
    /// <param name="cantidad">Cantidad a añadir (positiva) o restar (negativa)</param>
    /// <exception cref="ArgumentOutOfRangeException">Si la operación resultaría en stock negativo</exception>
    public void AjustarStock(int cantidad)
    {
        var nuevoStock = CantidadStock + cantidad;
        if (nuevoStock < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(cantidad),
                $"No hay suficiente stock. Stock actual: {CantidadStock}, solicitado: {-cantidad}");
        }

        CantidadStock = nuevoStock;
    }

    /// <summary>
    /// Actualiza el precio de venta del producto.
    /// </summary>
    /// <param name="nuevoPrecio">Nuevo precio en COP</param>
    public void ActualizarPrecio(decimal nuevoPrecio)
    {
        if (nuevoPrecio <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(nuevoPrecio), "PrecioVentaCOP debe ser mayor a cero.");
        }

        PrecioVentaCOP = nuevoPrecio;
    }
}
