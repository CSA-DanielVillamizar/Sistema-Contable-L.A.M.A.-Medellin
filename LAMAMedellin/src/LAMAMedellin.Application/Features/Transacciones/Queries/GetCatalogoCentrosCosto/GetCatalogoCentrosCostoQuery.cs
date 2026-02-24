using MediatR;

namespace LAMAMedellin.Application.Features.Transacciones.Queries.GetCatalogoCentrosCosto;

public sealed record GetCatalogoCentrosCostoQuery : IRequest<List<CatalogoCentroCostoDto>>;
