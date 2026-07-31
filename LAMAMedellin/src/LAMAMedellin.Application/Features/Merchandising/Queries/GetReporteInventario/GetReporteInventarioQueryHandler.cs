using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Queries.GetReporteInventario;

public sealed class GetReporteInventarioQueryHandler(
    IProductoRepository productoRepository,
    IMovimientoInventarioRepository movimientoRepository)
    : IRequestHandler<GetReporteInventarioQuery, ReporteInventarioDto>
{
    public async Task<ReporteInventarioDto> Handle(
        GetReporteInventarioQuery request,
        CancellationToken cancellationToken)
    {
        var productos = await productoRepository.GetAllAsync(cancellationToken);
        var movimientos = await movimientoRepository.GetAllAsync(cancellationToken);

        var lineas = productos
            .Select(p => ConstruirLinea(p, movimientos, request))
            .OrderBy(l => l.Nombre)
            .ToList();

        return new ReporteInventarioDto(
            request.Desde,
            request.Hasta,
            lineas.Sum(l => l.ValorInventario),
            lineas.Sum(l => l.UnidadesVendidas),
            lineas.Sum(l => l.IngresoVentas),
            lineas.Sum(l => l.CostoVentas),
            lineas.Sum(l => l.Utilidad),
            lineas.Count(l => l.BajoMinimo),
            lineas);
    }

    private static LineaReporteInventarioDto ConstruirLinea(
        Producto producto,
        IReadOnlyList<MovimientoInventario> todos,
        GetReporteInventarioQuery request)
    {
        var propios = todos.Where(m => m.ProductoId == producto.Id).ToList();

        // El costo promedio se calcula sobre TODAS las entradas historicas, no
        // solo las del rango: la mercancia vendida este mes pudo entrar el
        // anterior, y limitarlo al periodo daria un costo que no corresponde.
        var entradas = propios
            .Where(m => m.TipoMovimiento == TipoMovimientoInventario.Entrada && m.CostoUnitario.HasValue)
            .ToList();

        var unidadesCompradas = entradas.Sum(m => m.Cantidad);
        var costoPromedio = unidadesCompradas == 0
            ? 0m
            : decimal.Round(entradas.Sum(m => m.Cantidad * m.CostoUnitario!.Value) / unidadesCompradas, 2);

        var salidas = propios
            .Where(m => m.TipoMovimiento == TipoMovimientoInventario.Salida)
            .Where(m => request.Desde is null || DateOnly.FromDateTime(m.Fecha) >= request.Desde)
            .Where(m => request.Hasta is null || DateOnly.FromDateTime(m.Fecha) <= request.Hasta)
            .ToList();

        var unidadesVendidas = salidas.Sum(m => m.Cantidad);
        var ingresoVentas = unidadesVendidas * producto.PrecioVenta;
        var costoVentas = unidadesVendidas * costoPromedio;
        var utilidad = ingresoVentas - costoVentas;

        return new LineaReporteInventarioDto(
            producto.Id,
            producto.Nombre,
            producto.CodigoSKU,
            producto.CantidadEnStock,
            producto.CantidadMinima,
            // Es lo primero que necesita ver quien repone: no cuanto se vendio,
            // sino que esta por agotarse.
            producto.CantidadEnStock <= producto.CantidadMinima,
            producto.PrecioVenta,
            costoPromedio,
            producto.CantidadEnStock * costoPromedio,
            unidadesVendidas,
            ingresoVentas,
            costoVentas,
            utilidad,
            ingresoVentas == 0 ? 0m : decimal.Round(utilidad / ingresoVentas * 100m, 1));
    }
}
