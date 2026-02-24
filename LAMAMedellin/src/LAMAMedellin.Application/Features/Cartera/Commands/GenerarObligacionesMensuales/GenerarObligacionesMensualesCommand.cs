using MediatR;

namespace LAMAMedellin.Application.Features.Cartera.Commands.GenerarObligacionesMensuales;

public sealed record GenerarObligacionesMensualesCommand(string Periodo) : IRequest<int>;
