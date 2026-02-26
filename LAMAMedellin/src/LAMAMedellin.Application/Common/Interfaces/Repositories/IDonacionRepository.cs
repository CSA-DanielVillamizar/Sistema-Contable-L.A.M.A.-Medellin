using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface IDonacionRepository
{
    Task<IReadOnlyList<Donacion>> GetAllWithDetallesAsync(CancellationToken cancellationToken = default);
    Task<Donacion?> GetByIdWithDetallesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExisteCodigoVerificacionAsync(string codigoVerificacion, CancellationToken cancellationToken = default);
    Task AddAsync(Donacion donacion, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
