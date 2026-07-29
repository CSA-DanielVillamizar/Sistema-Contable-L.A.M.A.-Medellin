using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Eventos.Queries.GetEventoById;

public sealed class GetEventoByIdQueryHandler(IEventoRepository eventoRepository)
    : IRequestHandler<GetEventoByIdQuery, EventoDetalleDto?>
{
    public async Task<EventoDetalleDto?> Handle(GetEventoByIdQuery request, CancellationToken cancellationToken)
    {
        var evento = await eventoRepository.GetByIdWithAsistenciasAsync(request.Id, cancellationToken);
        if (evento is null)
        {
            return null;
        }

        var asistencias = evento.Asistencias
            .Select(a => new AsistenciaDto(
                a.MiembroId,
                a.Miembro is null
                    ? string.Empty
                    : $"{a.Miembro.Nombres} {a.Miembro.Apellidos}".Trim(),
                a.Asistio,
                a.CuotaAplicadaCOP))
            .ToList();

        return new EventoDetalleDto(
            evento.Id,
            evento.Nombre,
            evento.Descripcion,
            evento.FechaProgramada,
            evento.LugarEncuentro,
            evento.Destino,
            evento.TipoEvento.ToString(),
            evento.Estado.ToString(),
            evento.CuotaLogisticaCOP,
            asistencias);
    }
}
