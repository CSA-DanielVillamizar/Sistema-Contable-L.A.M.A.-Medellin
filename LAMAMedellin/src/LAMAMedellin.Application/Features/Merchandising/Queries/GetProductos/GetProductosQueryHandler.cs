using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Queries.GetProductos;

public sealed class GetProductosQueryHandler(IProductoRepository productoRepository)
    : IRequestHandler<GetProductosQuery, List<ProductoDto>>
{
    public async Task<List<ProductoDto>> Handle(GetProductosQuery request, CancellationToken cancellationToken)
    {
        var productos = await productoRepository.GetAllAsync(cancellationToken);

        return productos
            .Select(producto => new ProductoDto(
                producto.Id,
                producto.Nombre,
                producto.SKU,
                producto.PrecioVentaCOP,
                producto.CantidadStock,
                producto.CuentaContableIngresoId,
                producto.CuentaContableIngreso?.Codigo ?? string.Empty,
                producto.CuentaContableIngreso?.Descripcion ?? string.Empty))
            .ToList();
    }
}
