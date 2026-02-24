using MediatR;

namespace LAMAMedellin.Application.Features.Transacciones.Queries.GetCatalogoBancos;

public sealed record GetCatalogoBancosQuery : IRequest<List<CatalogoBancoDto>>;
