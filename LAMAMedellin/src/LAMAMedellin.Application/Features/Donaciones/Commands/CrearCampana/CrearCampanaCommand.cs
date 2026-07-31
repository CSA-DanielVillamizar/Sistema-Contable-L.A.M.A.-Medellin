using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Commands.CrearCampana;

/// <summary>Alta de una campana de donacion con meta y vigencia (historia 2-1).</summary>
public sealed record CrearCampanaCommand(
    string Nombre,
    string Descripcion,
    decimal MetaCOP,
    DateOnly FechaInicio,
    DateOnly FechaFin) : IRequest<Guid>;
