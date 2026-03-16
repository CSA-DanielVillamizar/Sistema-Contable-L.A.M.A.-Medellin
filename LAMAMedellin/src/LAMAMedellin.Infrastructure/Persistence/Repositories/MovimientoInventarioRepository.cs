using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio concreto para operaciones sobre la entidad MovimientoInventario.
/// </summary>
public sealed class MovimientoInventarioRepository(LamaDbContext context) : IMovimientoInventarioRepository
{
    public async Task<IReadOnlyList<MovimientoInventario>> GetByProductoIdAsync(Guid productoId, CancellationToken cancellationToken = default)
    {
        return await context.MovimientosInventario
            .AsNoTracking()
            .Where(x => x.ProductoId == productoId)
            .OrderByDescending(x => x.Fecha)
            .ToListAsync(cancellationToken);
    }

    public Task<MovimientoInventario?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.MovimientosInventario
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(MovimientoInventario movimiento, CancellationToken cancellationToken = default)
    {
        await context.MovimientosInventario.AddAsync(movimiento, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
