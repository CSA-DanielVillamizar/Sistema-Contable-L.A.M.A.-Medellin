using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Commands.ProcesarVenta;

public sealed record ProcesarVentaCommand(
    string NumeroFacturaInterna,
    Guid? CompradorId,
    MetodoPagoVenta MetodoPago,
    IReadOnlyList<ProcesarVentaDetalleDto> Detalles) : IRequest<Guid>;

public sealed record ProcesarVentaDetalleDto(
    Guid ArticuloId,
    int Cantidad);
