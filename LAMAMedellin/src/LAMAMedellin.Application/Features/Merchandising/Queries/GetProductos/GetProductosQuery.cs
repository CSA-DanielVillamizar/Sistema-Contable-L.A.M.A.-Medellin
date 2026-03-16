using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Queries.GetProductos;

public sealed record GetProductosQuery : IRequest<List<ProductoDto>>;
