using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Commands.ActualizarCampana;

public sealed record ActualizarCampanaCommand(
    Guid Id,
    string Nombre,
    string Descripcion,
    decimal MetaCOP,
    DateOnly FechaInicio,
    DateOnly FechaFin) : IRequest;
