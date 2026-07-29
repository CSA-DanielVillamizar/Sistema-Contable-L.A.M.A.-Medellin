using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Eventos.Commands.EstablecerCuotaLogistica;

public sealed class EstablecerCuotaLogisticaCommandHandler(IEventoRepository eventoRepository)
    : IRequestHandler<EstablecerCuotaLogisticaCommand, Unit>
{
    public async Task<Unit> Handle(EstablecerCuotaLogisticaCommand request, CancellationToken cancellationToken)
    {
        var evento = await eventoRepository.GetByIdAsync(request.EventoId, cancellationToken);
        if (evento is null)
        {
            throw new ExcepcionNegocio("El evento indicado no existe.");
        }

        evento.EstablecerCuotaLogistica(request.CuotaLogisticaCOP);

        await eventoRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
