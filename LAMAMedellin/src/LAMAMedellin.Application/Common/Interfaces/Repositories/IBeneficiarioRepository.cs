using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface IBeneficiarioRepository
{
    Task<IReadOnlyList<Beneficiario>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Beneficiario?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Beneficiario?> GetByDocumentoAsync(string tipoDocumento, string numeroDocumento, CancellationToken cancellationToken = default);
    Task AddAsync(Beneficiario beneficiario, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
