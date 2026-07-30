using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using MediatR;

namespace LAMAMedellin.Application.Features.Proyectos.Commands.CrearActividad;

public sealed class CrearActividadCommandHandler(
    IActividadProyectoRepository actividadRepository,
    IProyectoSocialRepository proyectoRepository)
    : IRequestHandler<CrearActividadCommand, Guid>
{
    public async Task<Guid> Handle(CrearActividadCommand request, CancellationToken cancellationToken)
    {
        _ = await proyectoRepository.GetByIdAsync(request.ProyectoSocialId, cancellationToken)
            ?? throw new ExcepcionNegocio("El proyecto indicado no existe.");

        var actividad = new ActividadProyecto(
            request.ProyectoSocialId,
            request.Nombre,
            request.Descripcion,
            request.FechaInicioPlanificada,
            request.FechaFinPlanificada,
            request.PresupuestoAsignado,
            request.Responsable);

        await actividadRepository.AddAsync(actividad, cancellationToken);
        await actividadRepository.SaveChangesAsync(cancellationToken);

        return actividad.Id;
    }
}
