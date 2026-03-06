using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Tributario.Queries.GetReporteCalidadDatos;

public sealed class GetReporteCalidadDatosQueryHandler(ITributarioRepository tributarioRepository)
    : IRequestHandler<GetReporteCalidadDatosQuery, IReadOnlyList<InconsistenciaTributariaDto>>
{
    public async Task<IReadOnlyList<InconsistenciaTributariaDto>> Handle(GetReporteCalidadDatosQuery request, CancellationToken cancellationToken)
    {
        return await tributarioRepository.GetReporteCalidadDatosAsync(cancellationToken);
    }
}
