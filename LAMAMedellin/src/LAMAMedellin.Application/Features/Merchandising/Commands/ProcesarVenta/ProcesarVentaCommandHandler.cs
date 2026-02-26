using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Commands.ProcesarVenta;

public sealed class ProcesarVentaCommandHandler(
    IArticuloRepository articuloRepository,
    IVentaRepository ventaRepository)
    : IRequestHandler<ProcesarVentaCommand, Guid>
{
    public async Task<Guid> Handle(ProcesarVentaCommand request, CancellationToken cancellationToken)
    {
        if (request.Detalles.Count == 0)
        {
            throw new ExcepcionNegocio("La venta debe contener al menos un detalle.");
        }

        var idsArticulos = request.Detalles
            .Select(x => x.ArticuloId)
            .Distinct()
            .ToArray();

        var articulos = await articuloRepository.GetByIdsAsync(idsArticulos, cancellationToken);
        var articulosPorId = articulos.ToDictionary(x => x.Id);

        foreach (var articuloId in idsArticulos)
        {
            if (!articulosPorId.ContainsKey(articuloId))
            {
                throw new ExcepcionNegocio("Uno de los artículos indicados no existe.");
            }
        }

        decimal total = 0;
        var fechaVenta = DateTime.UtcNow;
        var venta = new Venta(
            request.NumeroFacturaInterna,
            fechaVenta,
            request.CompradorId,
            0,
            request.MetodoPago);

        foreach (var detalle in request.Detalles)
        {
            var articulo = articulosPorId[detalle.ArticuloId];
            articulo.ReducirStock(detalle.Cantidad);

            var subtotal = articulo.PrecioVenta * detalle.Cantidad;
            total += subtotal;

            var detalleVenta = new DetalleVenta(
                venta.Id,
                articulo.Id,
                detalle.Cantidad,
                articulo.PrecioVenta,
                subtotal);

            venta.AgregarDetalle(detalleVenta);
        }

        venta.AsignarTotal(total);

        await ventaRepository.AddAsync(venta, cancellationToken);
        await ventaRepository.SaveChangesAsync(cancellationToken);

        return venta.Id;
    }
}
