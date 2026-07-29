using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Tesoreria.Commands.RegistrarEgreso;

public sealed record RegistrarEgresoCommand(
    decimal Monto,
    string Concepto,
    Guid? TerceroId,
    Guid CuentaContableId,
    Guid BancoId,
    Guid CentroCostoId,
    MedioPago MedioPago) : IRequest<Guid>;
