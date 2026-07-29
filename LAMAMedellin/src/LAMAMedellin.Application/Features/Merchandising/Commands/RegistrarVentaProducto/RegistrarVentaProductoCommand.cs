using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Commands.RegistrarVentaProducto;

public sealed record RegistrarVentaProductoCommand(
    Guid ProductoId,
    int Cantidad,
    Guid BancoId,
    string Concepto,
    MedioPago MedioPago) : IRequest<Guid>;
