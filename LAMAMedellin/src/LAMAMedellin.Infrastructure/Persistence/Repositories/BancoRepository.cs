using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class BancoRepository(LamaDbContext dbContext) : IBancoRepository
{
    public Task<List<Banco>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Bancos
            .OrderBy(banco => banco.Nombre)
            .ToListAsync(cancellationToken);
    }

    public Task<Banco?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Bancos.FirstOrDefaultAsync(banco => banco.Id == id, cancellationToken);
    }

    public Task<Banco?> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        // Se restringe a cuentas activas: una cuenta dada de baja conserva su
        // historia pero no debe recibir movimientos nuevos.
        return dbContext.Bancos
            .Where(banco => banco.EsActivo)
            .OrderBy(banco => banco.Nombre)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalSaldoActualAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Bancos
            .Where(banco => banco.EsActivo)
            .SumAsync(banco => banco.SaldoActual, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
