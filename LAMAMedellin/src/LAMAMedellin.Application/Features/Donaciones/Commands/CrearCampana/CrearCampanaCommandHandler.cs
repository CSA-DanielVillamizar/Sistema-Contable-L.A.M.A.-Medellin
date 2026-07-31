using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Commands.CrearCampana;

public sealed class CrearCampanaCommandHandler(ICampanaDonacionRepository campanaRepository)
    : IRequestHandler<CrearCampanaCommand, Guid>
{
    public async Task<Guid> Handle(CrearCampanaCommand request, CancellationToken cancellationToken)
    {
        var campana = new CampanaDonacion(
            request.Nombre,
            request.Descripcion,
            request.MetaCOP,
            request.FechaInicio,
            request.FechaFin);

        await campanaRepository.AddAsync(campana, cancellationToken);
        await campanaRepository.SaveChangesAsync(cancellationToken);

        return campana.Id;
    }
}
