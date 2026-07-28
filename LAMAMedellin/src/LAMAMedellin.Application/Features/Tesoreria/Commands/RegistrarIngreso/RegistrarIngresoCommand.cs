using MediatR;

namespace LAMAMedellin.Application.Features.Tesoreria.Commands.RegistrarIngreso;

public sealed record RegistrarIngresoCommand(
    decimal Monto,
    string Concepto,
    Guid? TerceroId,
    Guid CuentaContableId,
    Guid BancoId,
    Guid CentroCostoId) : IRequest<Guid>;
