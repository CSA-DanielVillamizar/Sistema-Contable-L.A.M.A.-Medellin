using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Features.Tesoreria.Commands.RegistrarIngreso;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Commands.RegistrarVentaProducto;

public sealed class RegistrarVentaProductoCommandHandler(
    IProductoRepository productoRepository,
    IMovimientoInventarioRepository movimientoInventarioRepository,
    ICentroCostoRepository centroCostoRepository,
    ISender sender)
    : IRequestHandler<RegistrarVentaProductoCommand, Guid>
{
    public async Task<Guid> Handle(RegistrarVentaProductoCommand request, CancellationToken cancellationToken)
    {
        var producto = await productoRepository.GetByIdAsync(request.ProductoId, cancellationToken);
        if (producto is null)
        {
            throw new ExcepcionNegocio("El producto indicado no existe.");
        }

        var centroCosto = (await centroCostoRepository.GetAllAsync(cancellationToken))
            .FirstOrDefault(x => !x.IsDeleted);

        if (centroCosto is null)
        {
            throw new ExcepcionNegocio("No existe un centro de costo activo para registrar la venta en tesorería.");
        }

        // 1) Registrar salida de inventario
        producto.AjustarStock(-request.Cantidad);

        var conceptoVenta = request.Concepto.Trim();
        var movimiento = new MovimientoInventario(
            productoId: request.ProductoId,
            tipoMovimiento: TipoMovimientoInventario.Salida,
            cantidad: request.Cantidad,
            fecha: DateTime.UtcNow,
            concepto: $"Salida por venta: {conceptoVenta}",
            observaciones: null);

        await movimientoInventarioRepository.AddAsync(movimiento, cancellationToken);

        // 2) Calcular monto total de la venta
        var montoVenta = request.Cantidad * producto.PrecioVenta;

        // 3) Registrar ingreso en tesorería para conciliar automáticamente la venta
        await sender.Send(new RegistrarIngresoCommand(
            Monto: montoVenta,
            Concepto: $"Venta: {conceptoVenta}",
            TerceroId: null,
            CuentaContableId: producto.CuentaContableIngresoId,
            BancoId: request.BancoId,
            CentroCostoId: centroCosto.Id,
            MedioPago: request.MedioPago), cancellationToken);

        return movimiento.Id;
    }
}
