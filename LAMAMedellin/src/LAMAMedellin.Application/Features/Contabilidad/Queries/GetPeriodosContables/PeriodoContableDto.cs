using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetPeriodosContables;

public sealed record PeriodoContableDto(
    int Anio,
    int Mes,
    EstadoPeriodoContable Estado,
    DateTime? FechaValidacionTesoreria,
    string? ValidadoPor,
    DateTime? FechaCierre,
    string? CerradoPor);
