using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Tributario.Queries.GetReporteExogena;

public sealed class GetReporteExogenaQueryHandler(ITributarioRepository tributarioRepository)
    : IRequestHandler<GetReporteExogenaQuery, IReadOnlyList<ReporteExogenaDto>>
{
    public async Task<IReadOnlyList<ReporteExogenaDto>> Handle(GetReporteExogenaQuery request, CancellationToken cancellationToken)
    {
        return await tributarioRepository.GetReporteExogenaAsync(request.Anio, request.Mes, cancellationToken);
    }
}
