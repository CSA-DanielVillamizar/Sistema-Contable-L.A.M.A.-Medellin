using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Common.Interfaces.Services;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Commands.RegistrarVentaProducto;

public sealed class RegistrarVentaProductoCommandHandler(
    IProductoRepository productoRepository,
    IMovimientoInventarioRepository movimientoInventarioRepository,
    ICajaRepository cajaRepository,
    ICentroCostoRepository centroCostoRepository,
    ICuentaContableRepository cuentaContableRepository,
    IComprobanteRepository comprobanteRepository,
    ITransactionManager transactionManager)
    : IRequestHandler<RegistrarVentaProductoCommand, Guid>
{
    public async Task<Guid> Handle(RegistrarVentaProductoCommand request, CancellationToken cancellationToken)
    {
        return await transactionManager.ExecuteInTransactionAsync(async ct =>
        {
            var producto = await productoRepository.GetByIdAsync(request.ProductoId, ct);
            if (producto is null)
            {
                throw new ExcepcionNegocio("El producto indicado no existe.");
            }

            var caja = await cajaRepository.GetByIdAsync(request.CajaId, ct);
            if (caja is null)
            {
                throw new ExcepcionNegocio("La caja indicada no existe.");
            }

            var centroCosto = await centroCostoRepository.GetByIdAsync(request.CentroCostoId, ct);
            if (centroCosto is null)
            {
                throw new ExcepcionNegocio("El centro de costo indicado no existe.");
            }

            var cuentaIngresoProducto = await cuentaContableRepository.GetByIdAsync(producto.CuentaContableIngresoId, ct);
            if (cuentaIngresoProducto is null)
            {
                throw new ExcepcionNegocio("La cuenta contable de ingreso del producto no existe.");
            }

            if (!cuentaIngresoProducto.PermiteMovimiento)
            {
                throw new ExcepcionNegocio("La cuenta contable de ingreso del producto no permite movimiento.");
            }

            var cuentaCaja = await cuentaContableRepository.GetByIdAsync(caja.CuentaContableId, ct);
            if (cuentaCaja is null)
            {
                throw new ExcepcionNegocio("La cuenta contable asociada a la caja no existe.");
            }

            if (!cuentaCaja.PermiteMovimiento)
            {
                throw new ExcepcionNegocio("La cuenta contable asociada a la caja no permite movimiento.");
            }

            producto.AjustarStock(-request.Cantidad);

            var movimiento = new MovimientoInventario(
                request.ProductoId,
                -request.Cantidad,
                TipoMovimientoInventario.Salida,
                request.Fecha,
                request.Observaciones);

            await movimientoInventarioRepository.AddAsync(movimiento, ct);

            var valorTotalVenta = request.Cantidad * producto.PrecioVentaCOP;
            caja.AplicarIngreso(valorTotalVenta);

            var comprobante = new Comprobante(
                GenerarNumeroConsecutivo(),
                request.Fecha,
                TipoComprobante.Ingreso,
                $"Venta producto {producto.SKU} - {producto.Nombre}",
                EstadoComprobante.Asentado);

            comprobante.AgregarAsiento(AsientoContable.Crear(
                comprobante.Id,
                caja.CuentaContableId,
                null,
                request.CentroCostoId,
                valorTotalVenta,
                0m,
                "Venta merchandising - debito caja"));

            comprobante.AgregarAsiento(AsientoContable.Crear(
                comprobante.Id,
                producto.CuentaContableIngresoId,
                null,
                request.CentroCostoId,
                0m,
                valorTotalVenta,
                "Venta merchandising - credito ingreso"));

            await comprobanteRepository.AddAsync(comprobante, ct);

            return comprobante.Id;
        }, cancellationToken);
    }

    private static string GenerarNumeroConsecutivo() =>
        $"VTA-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
}
