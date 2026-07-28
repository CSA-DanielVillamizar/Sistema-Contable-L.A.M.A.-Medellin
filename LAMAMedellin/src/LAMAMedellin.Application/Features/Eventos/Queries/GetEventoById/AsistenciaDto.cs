namespace LAMAMedellin.Application.Features.Eventos.Queries.GetEventoById;

public sealed record AsistenciaDto(
    Guid MiembroId,
    string NombreMiembro,
    bool Asistio,
    decimal? CuotaAplicadaCOP);
