using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Commands.ActualizarCampana;

public sealed class ActualizarCampanaCommandHandler(ICampanaDonacionRepository campanaRepository)
    : IRequestHandler<ActualizarCampanaCommand>
{
    public async Task Handle(ActualizarCampanaCommand request, CancellationToken cancellationToken)
    {
        var campana = await campanaRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ExcepcionNegocio("La campana indicada no existe.");

        campana.ActualizarDatos(
            request.Nombre,
            request.Descripcion,
            request.MetaCOP,
            request.FechaInicio,
            request.FechaFin);

        await campanaRepository.SaveChangesAsync(cancellationToken);
    }
}
