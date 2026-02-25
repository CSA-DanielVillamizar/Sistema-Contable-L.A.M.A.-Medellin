using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface IComprobanteRepository
{
    Task<Comprobante?> GetByIdWithAsientosAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Comprobante comprobante, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
