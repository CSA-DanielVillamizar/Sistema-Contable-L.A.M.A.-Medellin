using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class EgresoRepository(LamaDbContext context) : IEgresoRepository
{
    public async Task<IReadOnlyList<Egreso>> GetAllWithDetallesAsync(CancellationToken cancellationToken = default)
    {
        return await context.Egresos
            .AsNoTracking()
            .Include(x => x.Banco)
            .Include(x => x.CuentaContable)
            .OrderByDescending(x => x.Fecha)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Egreso>> GetByFechaRangoAsync(
        DateTime fechaInicio,
        DateTime fechaFin,
        CancellationToken cancellationToken = default)
    {
        return await context.Egresos
            .AsNoTracking()
            .Where(x => x.Fecha >= fechaInicio && x.Fecha <= fechaFin)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Egreso egreso, CancellationToken cancellationToken = default)
    {
        await context.Egresos.AddAsync(egreso, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
