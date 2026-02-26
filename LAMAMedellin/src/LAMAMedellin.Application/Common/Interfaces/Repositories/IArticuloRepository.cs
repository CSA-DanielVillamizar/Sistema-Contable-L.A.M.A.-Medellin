using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface IArticuloRepository
{
    Task<IReadOnlyList<Articulo>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Articulo>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task AddAsync(Articulo articulo, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
