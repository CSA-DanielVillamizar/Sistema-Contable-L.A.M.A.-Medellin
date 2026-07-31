using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class MapeoContableRepository(LamaDbContext dbContext) : IMapeoContableRepository
{
    public Task<List<MapeoContable>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.MapeosContables
            .OrderBy(m => m.TipoOperacion)
            .ToListAsync(cancellationToken);
    }

    public Task<MapeoContable?> GetPorOperacionAsync(
        TipoOperacionContable tipoOperacion,
        CancellationToken cancellationToken = default)
    {
        return dbContext.MapeosContables
            .FirstOrDefaultAsync(m => m.TipoOperacion == tipoOperacion, cancellationToken);
    }

    public async Task AddAsync(MapeoContable mapeo, CancellationToken cancellationToken = default)
    {
        await dbContext.MapeosContables.AddAsync(mapeo, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
