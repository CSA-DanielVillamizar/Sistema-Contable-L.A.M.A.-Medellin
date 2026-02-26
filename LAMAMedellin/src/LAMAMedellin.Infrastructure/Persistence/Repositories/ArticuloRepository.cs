using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class ArticuloRepository(LamaDbContext context) : IArticuloRepository
{
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
}
