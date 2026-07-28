using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Eventos.Commands.CreateEvento;

public sealed record CreateEventoCommand(
    string Nombre,
    string Descripcion,
    DateTime FechaProgramada,
    string LugarEncuentro,
    TipoEvento TipoEvento,
    string? Destino,
    decimal? CuotaLogisticaCOP) : IRequest<Guid>;
