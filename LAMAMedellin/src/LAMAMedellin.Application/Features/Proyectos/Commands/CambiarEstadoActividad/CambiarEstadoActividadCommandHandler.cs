using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Proyectos.Commands.CambiarEstadoActividad;

public sealed class CambiarEstadoActividadCommandHandler(IActividadProyectoRepository actividadRepository)
    : IRequestHandler<CambiarEstadoActividadCommand>
{
    public async Task Handle(CambiarEstadoActividadCommand request, CancellationToken cancellationToken)
    {
        var actividad = await actividadRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ExcepcionNegocio("La actividad indicada no existe.");

        actividad.CambiarEstado(request.Estado);

        await actividadRepository.SaveChangesAsync(cancellationToken);
    }
}
