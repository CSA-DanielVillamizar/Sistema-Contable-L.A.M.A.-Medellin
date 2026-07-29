using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Eventos.Commands.ActualizarEvento;

/// <summary>
/// El frontend ya ofrecia editar eventos, pero no existia ni el endpoint ni el
/// caso de uso: el boton respondia 404.
/// </summary>
public sealed class ActualizarEventoCommandHandler(IEventoRepository eventoRepository)
    : IRequestHandler<ActualizarEventoCommand, Unit>
{
    public async Task<Unit> Handle(ActualizarEventoCommand request, CancellationToken cancellationToken)
    {
        var evento = await eventoRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ExcepcionNegocio("El evento indicado no existe.");

        evento.ActualizarDatos(
            request.Nombre,
            request.Descripcion,
            request.FechaProgramada,
            request.LugarEncuentro,
            request.TipoEvento,
            request.Destino);

        await eventoRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
