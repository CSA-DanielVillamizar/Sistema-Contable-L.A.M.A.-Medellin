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

        // Una cuenta inactiva no puede recibir movimientos: ofrecerla en el
        // desplegable solo consigue que el registro falle al guardar.
        return bancos
            .Where(banco => !banco.IsDeleted && banco.EsActivo)
            .OrderBy(banco => banco.Nombre)
            .Select(banco => new CatalogoBancoDto(
                banco.Id,
                banco.Nombre,
                banco.NumeroCuenta))
            .ToList();
    }
}
