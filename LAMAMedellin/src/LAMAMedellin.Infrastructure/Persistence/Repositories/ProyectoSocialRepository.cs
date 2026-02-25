using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class ProyectoSocialRepository(LamaDbContext dbContext) : IProyectoSocialRepository
{
    public async Task<IReadOnlyList<ProyectoSocial>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.ProyectosSociales
            .OrderBy(p => p.Nombre)
            .ToListAsync(cancellationToken);
    }

    public Task<ProyectoSocial?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.ProyectosSociales.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task AddAsync(ProyectoSocial proyectoSocial, CancellationToken cancellationToken = default)
    {
        return dbContext.ProyectosSociales.AddAsync(proyectoSocial, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
