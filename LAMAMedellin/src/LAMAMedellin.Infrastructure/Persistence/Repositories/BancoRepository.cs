using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class BancoRepository(LamaDbContext dbContext) : IBancoRepository
{
    public Task<Banco?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Bancos.FirstOrDefaultAsync(banco => banco.Id == id, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
