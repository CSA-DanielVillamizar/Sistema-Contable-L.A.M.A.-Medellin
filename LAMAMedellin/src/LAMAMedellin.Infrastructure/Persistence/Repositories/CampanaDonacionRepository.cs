using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class CampanaDonacionRepository(LamaDbContext dbContext) : ICampanaDonacionRepository
{
    public Task<List<CampanaDonacion>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.CampanasDonacion
            .OrderByDescending(c => c.FechaInicio)
            .ToListAsync(cancellationToken);
    }

    public Task<CampanaDonacion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.CampanasDonacion.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task AddAsync(CampanaDonacion campana, CancellationToken cancellationToken = default)
    {
        await dbContext.CampanasDonacion.AddAsync(campana, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
