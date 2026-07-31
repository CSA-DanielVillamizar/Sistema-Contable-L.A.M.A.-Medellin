using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Eventos.Commands.ActualizarEvento;

public sealed record ActualizarEventoCommand(
    Guid Id,
    string Nombre,
    string Descripcion,
    DateTime FechaProgramada,
    string LugarEncuentro,
    TipoEvento TipoEvento,
    string? Destino) : IRequest<Unit>;
