using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Commands.CambiarEstadoCampana;

public sealed class CambiarEstadoCampanaCommandHandler(ICampanaDonacionRepository campanaRepository)
    : IRequestHandler<CambiarEstadoCampanaCommand>
{
    public async Task Handle(CambiarEstadoCampanaCommand request, CancellationToken cancellationToken)
    {
        var campana = await campanaRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ExcepcionNegocio("La campana indicada no existe.");

        if (request.Activa)
        {
            campana.Reabrir();
        }
        else
        {
            campana.Cerrar();
        }

        await campanaRepository.SaveChangesAsync(cancellationToken);
    }
}
