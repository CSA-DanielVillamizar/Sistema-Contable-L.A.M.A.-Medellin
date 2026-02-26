using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class DonanteRepository : IDonanteRepository
{
    private readonly LamaDbContext _context;

    public DonanteRepository(LamaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Donante donante, CancellationToken cancellationToken)
    {
        await _context.Donantes.AddAsync(donante, cancellationToken);
    }

    public Task<Donante?> GetByDocumentoAsync(TipoDocumentoDonante tipoDocumento, string numeroDocumento, CancellationToken cancellationToken)
    {
        return _context.Donantes
            .FirstOrDefaultAsync(x => x.TipoDocumento == tipoDocumento && x.NumeroDocumento == numeroDocumento, cancellationToken);
    }

    public Task<Donante?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Donantes
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Donante>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Donantes
            .AsNoTracking()
            .OrderBy(x => x.NombreORazonSocial)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
