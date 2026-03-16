using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Commands.CrearProducto;

/// <summary>
/// Handler para el comando CrearProducto.
/// Crea un nuevo producto con stock inicial de 0.
/// </summary>
public sealed class CrearProductoCommandHandler(
    IProductoRepository productoRepository,
    ICuentaContableRepository cuentaContableRepository)
    : IRequestHandler<CrearProductoCommand, Guid>
{
    public async Task<Guid> Handle(CrearProductoCommand request, CancellationToken cancellationToken)
    {
        // Validar que la cuenta contable existe y permite movimiento
        var cuentaIngreso = await cuentaContableRepository.GetByIdAsync(request.CuentaContableIngresoId, cancellationToken);
        if (cuentaIngreso is null)
        {
            throw new ExcepcionNegocio("La cuenta contable de ingreso indicada no existe.");
        }

        if (!cuentaIngreso.PermiteMovimiento)
        {
            throw new ExcepcionNegocio("La cuenta contable de ingreso no permite movimiento.");
        }

        // Crear producto con cantidad inicial 0
        var producto = new Producto(
            nombre: request.Nombre,
            sku: request.SKU,
            precioVentaCOP: request.PrecioVentaCOP,
            cantidadStock: 0,
            cuentaContableIngresoId: request.CuentaContableIngresoId);

        await productoRepository.AddAsync(producto, cancellationToken);

        return producto.Id;
    }
}
