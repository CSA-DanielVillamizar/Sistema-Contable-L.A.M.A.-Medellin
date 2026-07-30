using MediatR;

namespace LAMAMedellin.Application.Features.Proyectos.Commands.CrearActividad;

public sealed record CrearActividadCommand(
    Guid ProyectoSocialId,
    string Nombre,
    string Descripcion,
    DateOnly FechaInicioPlanificada,
    DateOnly FechaFinPlanificada,
    decimal PresupuestoAsignado,
    string? Responsable = null) : IRequest<Guid>;
