using MediatR;

namespace LAMAMedellin.Application.Features.Proyectos.Commands.ActualizarActividad;

public sealed record ActualizarActividadCommand(
    Guid Id,
    string Nombre,
    string Descripcion,
    DateOnly FechaInicioPlanificada,
    DateOnly FechaFinPlanificada,
    decimal PresupuestoAsignado,
    string? Responsable) : IRequest;
