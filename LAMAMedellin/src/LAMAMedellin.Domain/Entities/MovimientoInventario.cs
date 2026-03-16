using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

/// <summary>
/// Entidad que registra los movimientos de inventario de productos (entrada, salida o ajuste).
/// </summary>
public sealed class MovimientoInventario : BaseEntity
{
    /// <summary>ID del producto asociado al movimiento</summary>
    public Guid ProductoId { get; private set; }

    /// <summary>Cantidad de unidades movidas (positiva o negativa)</summary>
    public int Cantidad { get; private set; }

    /// <summary>Tipo de movimiento (Entrada, Salida, Ajuste)</summary>
    public TipoMovimientoInventario TipoMovimiento { get; private set; }

    /// <summary>Fecha del movimiento</summary>
    public DateTime Fecha { get; private set; }

    /// <summary>Observaciones adicionales sobre el movimiento</summary>
    public string? Observaciones { get; private set; }

    /// <summary>Navegación al producto</summary>
    public Producto? Producto { get; private set; }

#pragma warning disable CS8618
    private MovimientoInventario() { }
#pragma warning restore CS8618

    /// <summary>
    /// Constructor de MovimientoInventario.
    /// </summary>
    /// <param name="productoId">ID del producto</param>
    /// <param name="cantidad">Cantidad a mover</param>
    /// <param name="tipoMovimiento">Tipo de movimiento</param>
    /// <param name="fecha">Fecha del movimiento</param>
    /// <param name="observaciones">Observaciones opcionales</param>
    public MovimientoInventario(
        Guid productoId,
        int cantidad,
        TipoMovimientoInventario tipoMovimiento,
        DateTime fecha,
        string? observaciones = null)
    {
        if (productoId == Guid.Empty)
        {
            throw new ArgumentException("ProductoId es obligatorio.", nameof(productoId));
        }

        if (cantidad == 0)
        {
            throw new ArgumentException("Cantidad no puede ser cero.", nameof(cantidad));
        }

        if (fecha > DateTime.UtcNow.AddSeconds(5))
        {
            throw new ArgumentException("Fecha no puede ser en el futuro.", nameof(fecha));
        }

        if (fecha.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Fecha debe estar en UTC.", nameof(fecha));
        }

        ProductoId = productoId;
        Cantidad = cantidad;
        TipoMovimiento = tipoMovimiento;
        Fecha = fecha;
        Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim();
    }
}
