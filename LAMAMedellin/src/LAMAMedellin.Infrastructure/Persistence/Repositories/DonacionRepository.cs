using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class DonacionRepository : IDonacionRepository
{
    private readonly LamaDbContext _context;

    public DonacionRepository(LamaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Donacion donacion, CancellationToken cancellationToken)
    {
        await _context.Donaciones.AddAsync(donacion, cancellationToken);
    }

    public Task<bool> ExisteCodigoVerificacionAsync(string codigoVerificacion, CancellationToken cancellationToken)
    {
        return _context.Donaciones
            .AnyAsync(x => x.CodigoVerificacion == codigoVerificacion, cancellationToken);
    }

    public async Task<IReadOnlyList<Donacion>> GetAllWithDetallesAsync(CancellationToken cancellationToken)
    {
        return await _context.Donaciones
            .AsNoTracking()
            .Include(x => x.Donante)
            .OrderByDescending(x => x.Fecha)
            .ToListAsync(cancellationToken);
    }

    public Task<Donacion?> GetByIdWithDetallesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Donaciones
            .AsNoTracking()
            .Include(x => x.Donante)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
