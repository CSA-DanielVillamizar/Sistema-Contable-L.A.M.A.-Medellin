using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Proyectos.Queries.GetProyectosSociales;

public sealed record GetProyectosSocialesQuery : IRequest<IReadOnlyList<ProyectoSocialDto>>;

public sealed record ProyectoSocialDto(
    Guid Id,
    string Nombre,
    string Descripcion,
    DateTime FechaInicio,
    DateTime? FechaFin,
    decimal PresupuestoEstimado,
    string Estado);

public sealed class GetProyectosSocialesQueryHandler(IProyectoSocialRepository proyectoSocialRepository)
    : IRequestHandler<GetProyectosSocialesQuery, IReadOnlyList<ProyectoSocialDto>>
{
    public async Task<IReadOnlyList<ProyectoSocialDto>> Handle(GetProyectosSocialesQuery request, CancellationToken cancellationToken)
    {
        var proyectos = await proyectoSocialRepository.GetAllAsync(cancellationToken);

        return proyectos
            .Select(p => new ProyectoSocialDto(
                p.Id,
                p.Nombre,
                p.Descripcion,
                p.FechaInicio,
                p.FechaFin,
                p.PresupuestoEstimado,
                p.Estado.ToString()))
            .ToList();
    }
}
