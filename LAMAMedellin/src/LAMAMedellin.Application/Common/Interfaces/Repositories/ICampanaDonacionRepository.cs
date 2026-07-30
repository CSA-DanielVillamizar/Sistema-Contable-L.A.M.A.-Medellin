using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface ICampanaDonacionRepository
{
    Task<List<CampanaDonacion>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<CampanaDonacion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(CampanaDonacion campana, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
