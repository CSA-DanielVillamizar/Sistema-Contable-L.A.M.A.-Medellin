using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Common.Interfaces.Services;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Commands.RegistrarEntradaInventario;

/// <summary>
/// Handler para el comando RegistrarEntradaInventario.
/// Registra una entrada de inventario de forma atómica: ajusta stock del producto y crea registro de movimiento.
/// </summary>
public sealed class RegistrarEntradaInventarioCommandHandler(
    IProductoRepository productoRepository,
    IMovimientoInventarioRepository movimientoInventarioRepository,
    ITransactionManager transactionManager)
    : IRequestHandler<RegistrarEntradaInventarioCommand, Guid>
{
    public async Task<Guid> Handle(RegistrarEntradaInventarioCommand request, CancellationToken cancellationToken)
    {
        return await transactionManager.ExecuteInTransactionAsync(async ct =>
        {
            // Obtener producto
            var producto = await productoRepository.GetByIdAsync(request.ProductoId, ct);
            if (producto is null)
            {
                throw new ExcepcionNegocio("El producto indicado no existe.");
            }

            // Ajustar stock del producto (validación interna: cantidad debe ser positiva)
            producto.AjustarStock(request.Cantidad);

            // Registrar movimiento de inventario
            var movimiento = new MovimientoInventario(
                productoId: request.ProductoId,
                cantidad: request.Cantidad,
                tipoMovimiento: TipoMovimientoInventario.Entrada,
                fecha: request.Fecha,
                observaciones: request.Observaciones);

            await movimientoInventarioRepository.AddAsync(movimiento, ct);

            return movimiento.Id;
        }, cancellationToken);
    }
}
