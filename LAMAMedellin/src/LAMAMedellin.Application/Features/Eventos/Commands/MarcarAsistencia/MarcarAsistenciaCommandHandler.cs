using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using MediatR;

namespace LAMAMedellin.Application.Features.Eventos.Commands.MarcarAsistencia;

public sealed class MarcarAsistenciaCommandHandler(IEventoRepository eventoRepository)
    : IRequestHandler<MarcarAsistenciaCommand, Unit>
{
    public async Task<Unit> Handle(MarcarAsistenciaCommand request, CancellationToken cancellationToken)
    {
        var evento = await eventoRepository.GetByIdAsync(request.EventoId, cancellationToken);
        if (evento is null)
        {
            throw new ExcepcionNegocio("El evento indicado no existe.");
        }

        var asistencia = await eventoRepository.GetAsistenciaAsync(request.EventoId, request.MiembroId, cancellationToken);
        if (asistencia is null)
        {
            asistencia = new AsistenciaEvento(request.EventoId, request.MiembroId, request.Observaciones, evento.CuotaLogisticaCOP);
            await eventoRepository.AddAsistenciaAsync(asistencia, cancellationToken);
        }

        if (request.Asistio)
        {
            asistencia.MarcarAsistencia(request.Observaciones);
        }
        else
        {
            asistencia.MarcarInasistencia(request.Observaciones);
        }

        await eventoRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
