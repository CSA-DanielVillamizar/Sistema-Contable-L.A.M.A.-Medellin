using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Queries.GetReporteInventario;

/// <summary>
/// Reporte de inventario, ventas y utilidad (historia 4-3). El rango es
/// opcional; sin el toma todo el historico.
/// </summary>
public sealed record GetReporteInventarioQuery(DateOnly? Desde = null, DateOnly? Hasta = null)
    : IRequest<ReporteInventarioDto>;

/// <summary>Una linea del reporte: un producto con su movimiento del periodo.</summary>
public sealed record LineaReporteInventarioDto(
    Guid ProductoId,
    string Nombre,
    string CodigoSKU,
    int CantidadEnStock,
    int CantidadMinima,
    bool BajoMinimo,
    decimal PrecioVenta,
    decimal CostoPromedio,
    decimal ValorInventario,
    int UnidadesVendidas,
    decimal IngresoVentas,
    decimal CostoVentas,
    decimal Utilidad,
    decimal MargenPorcentaje);

public sealed record ReporteInventarioDto(
    DateOnly? Desde,
    DateOnly? Hasta,
    decimal ValorTotalInventario,
    int TotalUnidadesVendidas,
    decimal TotalIngresoVentas,
    decimal TotalCostoVentas,
    decimal UtilidadTotal,
    int ProductosBajoMinimo,
    IReadOnlyList<LineaReporteInventarioDto> Lineas);
