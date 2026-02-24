using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Transacciones.Queries.GetCatalogoBancos;

public sealed class GetCatalogoBancosQueryHandler(
    IBancoRepository bancoRepository)
    : IRequestHandler<GetCatalogoBancosQuery, List<CatalogoBancoDto>>
{
    public async Task<List<CatalogoBancoDto>> Handle(
        GetCatalogoBancosQuery request,
        CancellationToken cancellationToken)
    {
        var bancos = await bancoRepository.GetAllAsync(cancellationToken);

        return bancos
            .Where(banco => !banco.IsDeleted)
            .Select(banco => new CatalogoBancoDto(
                banco.Id,
                banco.NumeroCuenta))
            .ToList();
    }
}
