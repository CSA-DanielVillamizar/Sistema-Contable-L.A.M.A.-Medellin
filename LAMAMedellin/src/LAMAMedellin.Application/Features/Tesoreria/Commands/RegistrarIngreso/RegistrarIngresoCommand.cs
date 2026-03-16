using MediatR;

namespace LAMAMedellin.Application.Features.Tesoreria.Commands.RegistrarIngreso;

public sealed record RegistrarIngresoCommand(
    DateTime Fecha,
    decimal Monto,
    string Concepto,
    Guid? TerceroId,
    Guid CuentaContableId,
    Guid CajaId,
    Guid CentroCostoId) : IRequest<Guid>;
