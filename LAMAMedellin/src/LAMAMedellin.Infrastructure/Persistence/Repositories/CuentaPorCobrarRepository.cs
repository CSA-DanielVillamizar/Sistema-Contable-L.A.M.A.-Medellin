using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class CuentaPorCobrarRepository(LamaDbContext dbContext) : ICuentaPorCobrarRepository
{
    public Task<bool> ExistePorMiembroYPeriodoAsync(Guid miembroId, string periodo, CancellationToken cancellationToken = default)
    {
        return dbContext.CuentasPorCobrar
            .AnyAsync(c => c.MiembroId == miembroId && c.Periodo == periodo, cancellationToken);
    }

    public async Task<List<CuentaPorCobrar>> GetByEstadoAsync(EstadoCuentaPorCobrar estado, CancellationToken cancellationToken = default)
    {
        return await dbContext.CuentasPorCobrar
            .Include(c => c.Miembro)
            .Where(c => c.Estado == estado)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<CuentaPorCobrar?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.CuentasPorCobrar
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task AddRangeAsync(IEnumerable<CuentaPorCobrar> cuentasPorCobrar, CancellationToken cancellationToken = default)
    {
        return dbContext.CuentasPorCobrar
            .AddRangeAsync(cuentasPorCobrar, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
