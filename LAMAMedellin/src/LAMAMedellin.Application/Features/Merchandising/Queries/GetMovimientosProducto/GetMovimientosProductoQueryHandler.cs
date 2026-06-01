using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Queries.GetMovimientosProducto;

public sealed class GetMovimientosProductoQueryHandler(IMovimientoInventarioRepository movimientoInventarioRepository)
    : IRequestHandler<GetMovimientosProductoQuery, List<MovimientoInventarioDto>>
{
    public async Task<List<MovimientoInventarioDto>> Handle(GetMovimientosProductoQuery request, CancellationToken cancellationToken)
    {
        var movimientos = await movimientoInventarioRepository.GetByProductoIdAsync(request.ProductoId, cancellationToken);

        return movimientos
            .Select(movimiento => new MovimientoInventarioDto(
                movimiento.Id,
                movimiento.ProductoId,
                (int)movimiento.TipoMovimiento,
                movimiento.TipoMovimiento.ToString(),
                movimiento.Cantidad,
                movimiento.Fecha,
                movimiento.Concepto,
                movimiento.Observaciones))
            .ToList();
    }
}
