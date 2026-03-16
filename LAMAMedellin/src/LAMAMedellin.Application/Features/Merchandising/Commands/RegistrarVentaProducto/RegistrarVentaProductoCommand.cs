using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Commands.RegistrarVentaProducto;

public sealed record RegistrarVentaProductoCommand(
    Guid ProductoId,
    int Cantidad,
    Guid CajaId,
    Guid CentroCostoId,
    DateTime Fecha,
    string? Observaciones = null) : IRequest<Guid>;
