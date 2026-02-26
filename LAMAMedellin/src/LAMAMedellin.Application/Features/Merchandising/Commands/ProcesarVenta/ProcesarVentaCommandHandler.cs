using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Features.Contabilidad.Commands.RegistrarComprobante;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Commands.ProcesarVenta;

public sealed class ProcesarVentaCommandHandler(
    IArticuloRepository articuloRepository,
    IVentaRepository ventaRepository,
    ICuentaContableRepository cuentaContableRepository,
    ICentroCostoRepository centroCostoRepository,
    ISender sender)
    : IRequestHandler<ProcesarVentaCommand, Guid>
{
    private const string CodigoCuentaCajaBancos = "110505";

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

        await RegistrarComprobanteVentaAsync(
            request,
            venta,
            total,
            articulosPorId,
            cancellationToken);

        return venta.Id;
    }

    private async Task RegistrarComprobanteVentaAsync(
        ProcesarVentaCommand request,
        Venta venta,
        decimal total,
        IReadOnlyDictionary<Guid, Articulo> articulosPorId,
        CancellationToken cancellationToken)
    {
        var cuentaIngresoIds = request.Detalles
            .Select(x => articulosPorId[x.ArticuloId].CuentaContableIngresoId)
            .Distinct()
            .ToArray();

        if (cuentaIngresoIds.Length != 1)
        {
            throw new ExcepcionNegocio("La venta contiene artículos con diferentes cuentas contables de ingreso y no puede contabilizarse automáticamente en un solo asiento de crédito.");
        }

        var cuentas = await cuentaContableRepository.GetAllAsync(cancellationToken);
        var cuentaCajaOBancos = cuentas.FirstOrDefault(x => x.Codigo == CodigoCuentaCajaBancos)
            ?? cuentas.FirstOrDefault(x => x.PermiteMovimiento && x.Naturaleza == NaturalezaCuenta.Debito)
            ?? throw new ExcepcionNegocio("No existe una cuenta contable de débito disponible para registrar el ingreso de la venta.");

        var centroCosto = (await centroCostoRepository.GetAllAsync(cancellationToken)).FirstOrDefault()
            ?? throw new ExcepcionNegocio("No existe un centro de costo configurado para registrar el comprobante de venta.");

        var descripcion = $"Venta merchandising {venta.NumeroFacturaInterna}";

        await sender.Send(
            new RegistrarComprobanteCommand(
                venta.Fecha,
                TipoComprobante.Ingreso,
                descripcion,
                [
                    new RegistrarAsientoComprobanteDto(
                        cuentaCajaOBancos.Id,
                        request.CompradorId,
                        centroCosto.Id,
                        total,
                        0,
                        venta.NumeroFacturaInterna),
                    new RegistrarAsientoComprobanteDto(
                        cuentaIngresoIds[0],
                        request.CompradorId,
                        centroCosto.Id,
                        0,
                        total,
                        venta.NumeroFacturaInterna)
                ]),
            cancellationToken);
    }
}
