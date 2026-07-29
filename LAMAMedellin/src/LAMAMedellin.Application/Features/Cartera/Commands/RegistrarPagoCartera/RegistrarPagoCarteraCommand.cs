using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Cartera.Commands.RegistrarPagoCartera;

public sealed record RegistrarPagoCarteraCommand(
    Guid CuentaPorCobrarId,
    decimal Monto,
    Guid BancoId,
    MedioPago MedioPago) : IRequest<Unit>;
