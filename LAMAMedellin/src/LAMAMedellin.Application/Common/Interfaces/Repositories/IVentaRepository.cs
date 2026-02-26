using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface IVentaRepository
{
    Task<IReadOnlyList<Venta>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Venta venta, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
