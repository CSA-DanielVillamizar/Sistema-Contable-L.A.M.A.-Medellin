using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetCatalogoCuentas;

public sealed class GetCatalogoCuentasQueryHandler(
    ICuentaContableRepository cuentaContableRepository)
    : IRequestHandler<GetCatalogoCuentasQuery, List<CuentaContableDto>>
{
    public async Task<List<CuentaContableDto>> Handle(
        GetCatalogoCuentasQuery request,
        CancellationToken cancellationToken)
    {
        var cuentas = request.SoloAsentables
            ? await cuentaContableRepository.GetAsentablesAsync(cancellationToken)
            : await cuentaContableRepository.GetAllAsync(cancellationToken);

        return cuentas
            .Select(c => new CuentaContableDto(
                c.Id,
                c.Codigo,
                c.Descripcion,
                (int)c.Nivel,
                c.Nivel.ToString(),
                c.Naturaleza.ToString(),
                c.PermiteMovimiento,
                c.ExigeTercero,
                c.CuentaPadreId))
            .ToList();
    }
}
