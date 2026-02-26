using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class ArticuloRepository(LamaDbContext context) : IArticuloRepository
{
    public async Task<IReadOnlyList<Articulo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Articulos
            .OrderBy(x => x.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Articulo>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        return await context.Articulos
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return context.Articulos
            .AnyAsync(x => x.SKU == sku, cancellationToken);
    }

    public Task AddAsync(Articulo articulo, CancellationToken cancellationToken = default)
    {
        return context.Articulos.AddAsync(articulo, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
