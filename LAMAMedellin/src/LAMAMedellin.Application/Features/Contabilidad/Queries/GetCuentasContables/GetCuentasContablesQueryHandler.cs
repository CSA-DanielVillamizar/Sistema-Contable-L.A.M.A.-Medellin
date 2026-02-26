using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetCuentasContables;

public sealed class GetCuentasContablesQueryHandler(ICuentaContableRepository cuentaContableRepository)
    : IRequestHandler<GetCuentasContablesQuery, IReadOnlyList<CuentaContableDto>>
{
    public async Task<IReadOnlyList<CuentaContableDto>> Handle(GetCuentasContablesQuery request, CancellationToken cancellationToken)
    {
        var cuentas = await cuentaContableRepository.GetAllAsync(cancellationToken);

        return cuentas
            .OrderBy(x => x.Codigo)
            .Select(x => new CuentaContableDto(
                x.Id,
                x.Codigo,
                x.Descripcion,
                x.Naturaleza.ToString().ToUpperInvariant(),
                x.PermiteMovimiento,
                x.ExigeTercero))
            .ToList();
    }
}
