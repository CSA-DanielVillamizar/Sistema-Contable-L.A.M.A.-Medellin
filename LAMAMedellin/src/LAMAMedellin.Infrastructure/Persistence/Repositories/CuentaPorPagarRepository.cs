using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class CuentaPorPagarRepository(LamaDbContext dbContext) : ICuentaPorPagarRepository
{
    public Task<List<CuentaPorPagar>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.CuentasPorPagar
            .OrderBy(c => c.FechaVencimiento)
            .ToListAsync(cancellationToken);
    }

    public Task<CuentaPorPagar?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.CuentasPorPagar.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task<bool> ExisteFacturaAsync(
        string nitProveedor,
        string numeroFactura,
        CancellationToken cancellationToken = default)
    {
        var nit = nitProveedor.Trim();
        var numero = numeroFactura.Trim();

        return dbContext.CuentasPorPagar
            .AnyAsync(c => c.NitProveedor == nit && c.NumeroFactura == numero, cancellationToken);
    }

    public async Task AddAsync(CuentaPorPagar cuenta, CancellationToken cancellationToken = default)
    {
        await dbContext.CuentasPorPagar.AddAsync(cuenta, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
