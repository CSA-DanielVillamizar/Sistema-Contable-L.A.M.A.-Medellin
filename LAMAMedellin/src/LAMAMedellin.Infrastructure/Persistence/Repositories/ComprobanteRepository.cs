using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class ComprobanteRepository : IComprobanteRepository
{
    private readonly LamaDbContext _context;

    public ComprobanteRepository(LamaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Comprobante comprobante, CancellationToken cancellationToken = default)
    {
        await _context.Comprobantes.AddAsync(comprobante, cancellationToken);
    }

    public Task<Comprobante?> GetByIdWithAsientosAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Comprobantes
            .Include(x => x.AsientosContables)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
