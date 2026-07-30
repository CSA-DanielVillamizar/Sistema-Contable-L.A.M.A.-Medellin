using MediatR;

namespace LAMAMedellin.Application.Features.CuentasPorPagar.Commands.RegistrarCuentaPorPagar;

/// <summary>
/// Registra una factura de proveedor pendiente de pago (historia 1-13).
/// Reconoce el gasto y el pasivo en el momento en que llega la factura, no
/// cuando se paga.
/// </summary>
public sealed record RegistrarCuentaPorPagarCommand(
    string NombreProveedor,
    string NitProveedor,
    string NumeroFactura,
    string Concepto,
    Guid CuentaContableGastoId,
    Guid CentroCostoId,
    DateOnly FechaEmision,
    DateOnly FechaVencimiento,
    decimal ValorTotal) : IRequest<Guid>;
