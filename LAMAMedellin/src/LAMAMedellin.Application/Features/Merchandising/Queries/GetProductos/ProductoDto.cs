namespace LAMAMedellin.Application.Features.Merchandising.Queries.GetProductos;

public sealed record ProductoDto(
    Guid Id,
    string Nombre,
    string SKU,
    decimal PrecioVentaCOP,
    int CantidadStock,
    Guid CuentaContableIngresoId,
    string CuentaContableIngresoCodigo,
    string CuentaContableIngresoDescripcion);
