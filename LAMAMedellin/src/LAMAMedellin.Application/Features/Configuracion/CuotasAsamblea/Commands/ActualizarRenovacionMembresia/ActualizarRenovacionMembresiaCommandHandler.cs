using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Configuracion.CuotasAsamblea.Commands.ActualizarRenovacionMembresia;

public sealed class ActualizarRenovacionMembresiaCommandHandler(ICuotaAsambleaRepository cuotaAsambleaRepository)
    : IRequestHandler<ActualizarRenovacionMembresiaCommand, Unit>
{
    public async Task<Unit> Handle(ActualizarRenovacionMembresiaCommand request, CancellationToken cancellationToken)
    {
        var cuota = await cuotaAsambleaRepository.GetByAnioAsync(request.Anio, cancellationToken);
        if (cuota is null)
        {
            throw new ExcepcionNegocio($"No existe configuración de cuota asamblea para el año {request.Anio}.");
        }

        cuota.ActualizarRenovacionMembresiaUSD(request.RenovacionMembresiaUSD);

        await cuotaAsambleaRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
