using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface ITransaccionRepository
{
    Task<Transaccion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Transaccion>> GetAllWithDetallesAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Transaccion transaccion, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
