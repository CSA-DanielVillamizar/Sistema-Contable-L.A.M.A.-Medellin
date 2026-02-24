using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface IBancoRepository
{
    Task<List<Banco>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Banco?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Banco?> GetDefaultAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
