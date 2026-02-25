using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class BeneficiarioRepository(LamaDbContext dbContext) : IBeneficiarioRepository
{
    public async Task<IReadOnlyList<Beneficiario>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Beneficiarios
            .OrderBy(b => b.NombreCompleto)
            .ToListAsync(cancellationToken);
    }

    public Task<Beneficiario?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Beneficiarios.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public Task<Beneficiario?> GetByDocumentoAsync(string tipoDocumento, string numeroDocumento, CancellationToken cancellationToken = default)
    {
        return dbContext.Beneficiarios.FirstOrDefaultAsync(
            b => b.TipoDocumento == tipoDocumento && b.NumeroDocumento == numeroDocumento,
            cancellationToken);
    }

    public Task AddAsync(Beneficiario beneficiario, CancellationToken cancellationToken = default)
    {
        return dbContext.Beneficiarios.AddAsync(beneficiario, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
