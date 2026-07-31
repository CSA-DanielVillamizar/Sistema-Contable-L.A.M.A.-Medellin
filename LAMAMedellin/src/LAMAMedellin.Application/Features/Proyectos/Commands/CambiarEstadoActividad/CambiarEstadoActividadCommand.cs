using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Proyectos.Commands.CambiarEstadoActividad;

public sealed record CambiarEstadoActividadCommand(Guid Id, EstadoActividadProyecto Estado) : IRequest;
