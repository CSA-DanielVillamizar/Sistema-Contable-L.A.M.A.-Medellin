using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class CuentaPorCobrarRepository(LamaDbContext dbContext) : ICuentaPorCobrarRepository
{
    public Task<bool> ExisteParaMiembroYPeriodoAsync(
        Guid miembroId,
        Guid conceptoCobroId,
        string periodo,
        CancellationToken cancellationToken = default)
    {
        return dbContext.CuentasPorCobrar
            .AnyAsync(
                c => c.MiembroId == miembroId
                     && c.ConceptoCobroId == conceptoCobroId
                     && c.Periodo == periodo,
                cancellationToken);
    }

    public async Task<List<CuentaPorCobrar>> GetPendientesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.CuentasPorCobrar
            .Include(c => c.Miembro)
            .Where(c => c.SaldoPendiente > 0 && c.Estado != EstadoCuentaPorCobrar.Anulada)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<CuentaPorCobrar?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.CuentasPorCobrar
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<CuentaPorCobrar>> GetCarteraEnMoraAsync(
        DateOnly fechaCorte,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.CuentasPorCobrar
            .AsNoTracking()
            .Include(c => c.Miembro)
            .Include(c => c.ConceptoCobro)
            .Where(c => c.SaldoPendiente > 0 && c.FechaVencimiento < fechaCorte)
            .OrderBy(c => c.FechaVencimiento)
            .ThenBy(c => c.Miembro!.Nombres)
            .ThenBy(c => c.Miembro!.Apellidos)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CuentaPorCobrar>> GetListadoAsync(
        EstadoCuentaPorCobrar? estado = null,
        Guid? miembroId = null,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.CuentasPorCobrar
            .Include(c => c.Miembro)
            .Include(c => c.ConceptoCobro)
            .AsNoTracking()
            .AsQueryable();

        if (estado.HasValue)
        {
            query = query.Where(c => c.Estado == estado.Value);
        }

        if (miembroId.HasValue)
        {
            query = query.Where(c => c.MiembroId == miembroId.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(CuentaPorCobrar cuentaPorCobrar, CancellationToken cancellationToken = default)
    {
        await dbContext.CuentasPorCobrar.AddAsync(cuentaPorCobrar, cancellationToken);
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
