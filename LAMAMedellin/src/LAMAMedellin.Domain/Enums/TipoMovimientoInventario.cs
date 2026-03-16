namespace LAMAMedellin.Domain.Enums;

/// <summary>
/// Tipos de movimiento de inventario.
/// </summary>
public enum TipoMovimientoInventario
{
    /// <summary>Entrada de mercancía al inventario</summary>
    Entrada = 1,

    /// <summary>Salida de mercancía del inventario</summary>
    Salida = 2,

    /// <summary>Ajuste manual de inventario</summary>
    Ajuste = 3,
}
