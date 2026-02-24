using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class TransaccionRepository(LamaDbContext dbContext) : ITransaccionRepository
{
    public Task<Transaccion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Transacciones
            .Include(transaccion => transaccion.Banco)
            .Include(transaccion => transaccion.CentroCosto)
            .FirstOrDefaultAsync(transaccion => transaccion.Id == id, cancellationToken);
    }

    public Task<List<Transaccion>> GetAllWithDetallesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Transacciones
            .AsNoTracking()
            .Include(transaccion => transaccion.Banco)
            .Include(transaccion => transaccion.CentroCosto)
            .OrderByDescending(transaccion => transaccion.Fecha)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Transaccion transaccion, CancellationToken cancellationToken = default)
    {
        await dbContext.Transacciones.AddAsync(transaccion, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
