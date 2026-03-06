namespace LAMAMedellin.Application.Features.Tributario.Queries.GetReporteCalidadDatos;

public sealed record InconsistenciaTributariaDto(
    string TerceroId,
    string NombreObtenido,
    string TipoRelacion,
    string DescripcionInconsistencia);
