using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface IMiembroRepository
{
    Task<IReadOnlyList<Miembro>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Miembro>> GetActivosAsync(CancellationToken cancellationToken = default);
    Task<Miembro?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Miembro?> GetByDocumentoAsync(string documento, CancellationToken cancellationToken = default);
    Task<Miembro?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(Miembro miembro, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
