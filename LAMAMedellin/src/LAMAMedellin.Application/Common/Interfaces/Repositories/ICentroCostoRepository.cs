using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface ICentroCostoRepository
{
    Task<List<CentroCosto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CentroCosto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
