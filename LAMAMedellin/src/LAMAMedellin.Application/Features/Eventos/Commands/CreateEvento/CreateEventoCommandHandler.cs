using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using MediatR;

namespace LAMAMedellin.Application.Features.Eventos.Commands.CreateEvento;

public sealed class CreateEventoCommandHandler(IEventoRepository eventoRepository)
    : IRequestHandler<CreateEventoCommand, Guid>
{
    public async Task<Guid> Handle(CreateEventoCommand request, CancellationToken cancellationToken)
    {
        var evento = new Evento(
            request.Nombre,
            request.Descripcion,
            request.FechaProgramada,
            request.LugarEncuentro,
            request.TipoEvento,
            request.Destino,
            request.CuotaLogisticaCOP);

        await eventoRepository.AddAsync(evento, cancellationToken);
        await eventoRepository.SaveChangesAsync(cancellationToken);

        return evento.Id;
    }
}
