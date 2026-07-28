using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class PeriodoContableRepository(LamaDbContext dbContext) : IPeriodoContableRepository
{
    public async Task<IReadOnlyList<PeriodoContable>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.PeriodosContables
            .AsNoTracking()
            .OrderByDescending(p => p.Anio)
            .ThenByDescending(p => p.Mes)
            .ToListAsync(cancellationToken);
    }

    public Task<PeriodoContable?> GetPorAnioYMesAsync(
        int anio,
        int mes,
        CancellationToken cancellationToken = default)
    {
        return dbContext.PeriodosContables
            .FirstOrDefaultAsync(p => p.Anio == anio && p.Mes == mes, cancellationToken);
    }

    public Task<int> ContarComprobantesEnBorradorAsync(
        int anio,
        int mes,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Comprobantes
            .CountAsync(
                c => c.Fecha.Year == anio
                     && c.Fecha.Month == mes
                     && c.EstadoComprobante == EstadoComprobante.Borrador,
                cancellationToken);
    }

    public async Task AddAsync(PeriodoContable periodo, CancellationToken cancellationToken = default)
    {
        await dbContext.PeriodosContables.AddAsync(periodo, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
