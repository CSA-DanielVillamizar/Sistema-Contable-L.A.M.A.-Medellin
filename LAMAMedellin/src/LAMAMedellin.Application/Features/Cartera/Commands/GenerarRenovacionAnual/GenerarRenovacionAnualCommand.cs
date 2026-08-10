using MediatR;

namespace LAMAMedellin.Application.Features.Cartera.Commands.GenerarRenovacionAnual;

public sealed record GenerarRenovacionAnualCommand(int Anio, decimal TasaCambioUsada) : IRequest<int>;
