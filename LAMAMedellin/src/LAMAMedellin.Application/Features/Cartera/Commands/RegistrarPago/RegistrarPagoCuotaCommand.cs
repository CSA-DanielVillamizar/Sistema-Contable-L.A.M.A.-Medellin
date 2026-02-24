using MediatR;

namespace LAMAMedellin.Application.Features.Cartera.Commands.RegistrarPago;

public sealed record RegistrarPagoCuotaCommand(Guid CuentaPorCobrarId, decimal MontoCOP) : IRequest<Unit>;
