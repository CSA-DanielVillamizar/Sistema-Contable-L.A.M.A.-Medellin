using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class CentroCostoRepository(LamaDbContext dbContext) : ICentroCostoRepository
{
    public Task<List<CentroCosto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.CentrosCosto
            .OrderBy(centroCosto => centroCosto.Nombre)
            .ToListAsync(cancellationToken);
    }

    public Task<CentroCosto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.CentrosCosto.FirstOrDefaultAsync(centroCosto => centroCosto.Id == id, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
