using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class CuentaContableRepository(LamaDbContext dbContext) : ICuentaContableRepository
{
    public Task<List<CuentaContable>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.CuentasContables
            .OrderBy(c => c.Codigo)
            .ToListAsync(cancellationToken);
    }

    public Task<List<CuentaContable>> GetAsentablesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.CuentasContables
            .Where(c => c.PermiteMovimiento)
            .OrderBy(c => c.Codigo)
            .ToListAsync(cancellationToken);
    }

    public Task<CuentaContable?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return dbContext.CuentasContables
            .FirstOrDefaultAsync(c => c.Codigo == codigo, cancellationToken);
    }

    public Task<CuentaContable?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.CuentasContables
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
