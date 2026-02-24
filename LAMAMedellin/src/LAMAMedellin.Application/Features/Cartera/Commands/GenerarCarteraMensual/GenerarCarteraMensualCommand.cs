using MediatR;

namespace LAMAMedellin.Application.Features.Cartera.Commands.GenerarCarteraMensual;

public sealed record GenerarCarteraMensualCommand(string Periodo) : IRequest<int>;
