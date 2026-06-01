namespace LAMAMedellin.Application.Features.Merchandising.Queries.GetMovimientosProducto;

public sealed record MovimientoInventarioDto(
    Guid Id,
    Guid ProductoId,
    int TipoMovimiento,
    string TipoMovimientoNombre,
    int Cantidad,
    DateTime Fecha,
    string Concepto,
    string? Observaciones);
