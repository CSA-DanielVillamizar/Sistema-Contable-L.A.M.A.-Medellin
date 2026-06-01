using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Queries.GetMovimientosProducto;

public sealed record GetMovimientosProductoQuery(Guid ProductoId) : IRequest<List<MovimientoInventarioDto>>;
