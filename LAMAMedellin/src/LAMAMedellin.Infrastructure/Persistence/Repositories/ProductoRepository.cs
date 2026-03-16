using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio concreto para operaciones sobre la entidad Producto.
/// </summary>
public sealed class ProductoRepository(LamaDbContext context) : IProductoRepository
{
    public async Task<IReadOnlyList<Producto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Productos
            .AsNoTracking()
            .OrderBy(x => x.Nombre)
            .ToListAsync(cancellationToken);
    }

    public Task<Producto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.Productos
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<Producto?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return context.Productos
            .FirstOrDefaultAsync(x => x.SKU == sku.ToUpper(), cancellationToken);
    }

    public async Task AddAsync(Producto producto, CancellationToken cancellationToken = default)
    {
        await context.Productos.AddAsync(producto, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
