using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Queries.GetVentas;

public sealed record GetVentasQuery : IRequest<IReadOnlyList<VentaDto>>;

public sealed record VentaDto(
    Guid Id,
    DateTime Fecha,
    string NumeroFacturaInterna,
    string Cliente,
    string MetodoPago,
    decimal Total);

public sealed class GetVentasQueryHandler(IVentaRepository ventaRepository)
    : IRequestHandler<GetVentasQuery, IReadOnlyList<VentaDto>>
{
    public async Task<IReadOnlyList<VentaDto>> Handle(GetVentasQuery request, CancellationToken cancellationToken)
    {
        var ventas = await ventaRepository.GetAllAsync(cancellationToken);

        return ventas
            .OrderByDescending(x => x.Fecha)
            .Select(x => new VentaDto(
                x.Id,
                x.Fecha,
                x.NumeroFacturaInterna,
                x.CompradorId?.ToString() ?? "Consumidor final",
                x.MetodoPago.ToString(),
                x.Total))
            .ToList();
    }
}
