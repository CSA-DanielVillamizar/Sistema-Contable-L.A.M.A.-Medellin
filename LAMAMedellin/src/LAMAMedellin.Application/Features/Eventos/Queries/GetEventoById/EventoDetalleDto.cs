namespace LAMAMedellin.Application.Features.Eventos.Queries.GetEventoById;

public sealed record EventoDetalleDto(
    Guid Id,
    string Nombre,
    string Descripcion,
    DateTime FechaProgramada,
    string LugarEncuentro,
    string? Destino,
    string TipoEvento,
    string Estado,
    decimal? CuotaLogisticaCOP,
    IReadOnlyList<AsistenciaDto> Asistencias);
