namespace LAMAMedellin.Application.Features.Eventos.Queries.GetEventos;

public sealed record EventoDto(
    Guid Id,
    string Nombre,
    DateTime FechaProgramada,
    string TipoEvento,
    string Estado,
    decimal? CuotaLogisticaCOP);
