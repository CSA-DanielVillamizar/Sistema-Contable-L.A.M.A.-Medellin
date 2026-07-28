using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetPeriodosContables;

public sealed class GetPeriodosContablesQueryHandler(IPeriodoContableRepository periodoRepository)
    : IRequestHandler<GetPeriodosContablesQuery, IReadOnlyList<PeriodoContableDto>>
{
    public async Task<IReadOnlyList<PeriodoContableDto>> Handle(
        GetPeriodosContablesQuery request,
        CancellationToken cancellationToken)
    {
        var periodos = await periodoRepository.GetAllAsync(cancellationToken);

        return periodos
            .Select(p => new PeriodoContableDto(
                p.Anio,
                p.Mes,
                p.Estado,
                p.FechaValidacionTesoreria,
                p.ValidadoPor,
                p.FechaCierre,
                p.CerradoPor))
            .ToList();
    }
}
