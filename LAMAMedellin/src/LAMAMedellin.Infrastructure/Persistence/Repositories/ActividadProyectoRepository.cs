using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class ActividadProyectoRepository(LamaDbContext dbContext) : IActividadProyectoRepository
{
    public Task<List<ActividadProyecto>> GetPorProyectoAsync(
        Guid proyectoSocialId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.ActividadesProyecto
            .Where(a => a.ProyectoSocialId == proyectoSocialId)
            .OrderBy(a => a.FechaInicioPlanificada)
            .ToListAsync(cancellationToken);
    }

    public Task<List<ActividadProyecto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.ActividadesProyecto.ToListAsync(cancellationToken);
    }

    public Task<ActividadProyecto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.ActividadesProyecto.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task AddAsync(ActividadProyecto actividad, CancellationToken cancellationToken = default)
    {
        await dbContext.ActividadesProyecto.AddAsync(actividad, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
