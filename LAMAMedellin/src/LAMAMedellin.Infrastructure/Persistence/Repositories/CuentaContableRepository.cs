using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class CuentaContableRepository(LamaDbContext context) : ICuentaContableRepository
{
    public async Task<IReadOnlyList<CuentaContable>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        return await context.CuentasContables
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }
}
