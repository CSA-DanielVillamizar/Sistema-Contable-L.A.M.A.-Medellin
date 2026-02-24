using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Transacciones.Queries.GetCatalogoCentrosCosto;

public sealed class GetCatalogoCentrosCostoQueryHandler(
    ICentroCostoRepository centroCostoRepository)
    : IRequestHandler<GetCatalogoCentrosCostoQuery, List<CatalogoCentroCostoDto>>
{
    public async Task<List<CatalogoCentroCostoDto>> Handle(
        GetCatalogoCentrosCostoQuery request,
        CancellationToken cancellationToken)
    {
        var centrosCosto = await centroCostoRepository.GetAllAsync(cancellationToken);

        return centrosCosto
            .Where(centroCosto => !centroCosto.IsDeleted)
            .Select(centroCosto => new CatalogoCentroCostoDto(
                centroCosto.Id,
                centroCosto.Nombre))
            .ToList();
    }
}
