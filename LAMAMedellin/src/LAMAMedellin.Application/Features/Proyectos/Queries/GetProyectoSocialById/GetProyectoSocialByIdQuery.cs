using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Proyectos.Queries.GetProyectoSocialById;

public sealed record GetProyectoSocialByIdQuery(Guid Id) : IRequest<ProyectoSocialDto?>;

public sealed record ProyectoSocialDto(
    Guid Id,
    string Nombre,
    string Descripcion,
    DateTime FechaInicio,
    DateTime? FechaFin,
    decimal PresupuestoEstimado,
    string Estado);

public sealed class GetProyectoSocialByIdQueryHandler(IProyectoSocialRepository proyectoSocialRepository)
    : IRequestHandler<GetProyectoSocialByIdQuery, ProyectoSocialDto?>
{
    public async Task<ProyectoSocialDto?> Handle(GetProyectoSocialByIdQuery request, CancellationToken cancellationToken)
    {
        var proyecto = await proyectoSocialRepository.GetByIdAsync(request.Id, cancellationToken);
        if (proyecto is null)
        {
            return null;
        }

        return new ProyectoSocialDto(
            proyecto.Id,
            proyecto.Nombre,
            proyecto.Descripcion,
            proyecto.FechaInicio,
            proyecto.FechaFin,
            proyecto.PresupuestoEstimado,
            proyecto.Estado.ToString());
    }
}
