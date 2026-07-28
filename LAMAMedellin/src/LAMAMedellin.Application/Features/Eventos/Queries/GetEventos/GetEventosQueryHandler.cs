using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Eventos.Queries.GetEventos;

public sealed class GetEventosQueryHandler(IEventoRepository eventoRepository)
    : IRequestHandler<GetEventosQuery, IReadOnlyList<EventoDto>>
{
    public async Task<IReadOnlyList<EventoDto>> Handle(GetEventosQuery request, CancellationToken cancellationToken)
    {
        var eventos = await eventoRepository.GetAllAsync(cancellationToken);

        return eventos
            .Select(evento => new EventoDto(
                evento.Id,
                evento.Nombre,
                evento.FechaProgramada,
                evento.TipoEvento.ToString(),
                evento.Estado.ToString(),
                evento.CuotaLogisticaCOP))
            .ToList();
    }
}
