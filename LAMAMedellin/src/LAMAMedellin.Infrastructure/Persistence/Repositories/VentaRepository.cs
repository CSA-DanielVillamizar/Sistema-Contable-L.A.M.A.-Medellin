using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class VentaRepository(LamaDbContext context) : IVentaRepository
{
    public async Task<IReadOnlyList<Venta>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Ventas
            .OrderByDescending(x => x.Fecha)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Venta venta, CancellationToken cancellationToken = default)
    {
        await context.Ventas.AddAsync(venta, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
