using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Proyectos.Commands.ActualizarActividad;

public sealed class ActualizarActividadCommandHandler(IActividadProyectoRepository actividadRepository)
    : IRequestHandler<ActualizarActividadCommand>
{
    public async Task Handle(ActualizarActividadCommand request, CancellationToken cancellationToken)
    {
        var actividad = await actividadRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ExcepcionNegocio("La actividad indicada no existe.");

        actividad.ActualizarDatos(
            request.Nombre,
            request.Descripcion,
            request.FechaInicioPlanificada,
            request.FechaFinPlanificada,
            request.PresupuestoAsignado,
            request.Responsable);

        await actividadRepository.SaveChangesAsync(cancellationToken);
    }
}
