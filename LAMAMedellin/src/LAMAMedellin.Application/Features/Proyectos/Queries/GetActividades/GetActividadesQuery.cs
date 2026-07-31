using MediatR;

namespace LAMAMedellin.Application.Features.Proyectos.Queries.GetActividades;

public sealed record GetActividadesQuery(Guid ProyectoSocialId) : IRequest<IReadOnlyList<ActividadProyectoDto>>;

public sealed record ActividadProyectoDto(
    Guid Id,
    Guid ProyectoSocialId,
    string Nombre,
    string Descripcion,
    DateOnly FechaInicioPlanificada,
    DateOnly FechaFinPlanificada,
    decimal PresupuestoAsignado,
    int Estado,
    string NombreEstado,
    string? Responsable,
    bool EstaVencida);
