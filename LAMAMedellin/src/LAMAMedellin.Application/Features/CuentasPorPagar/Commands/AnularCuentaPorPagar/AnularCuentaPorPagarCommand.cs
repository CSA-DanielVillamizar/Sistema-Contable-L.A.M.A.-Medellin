using MediatR;

namespace LAMAMedellin.Application.Features.CuentasPorPagar.Commands.AnularCuentaPorPagar;

public sealed record AnularCuentaPorPagarCommand(Guid Id) : IRequest;
