using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

/// <summary>
/// Interfaz para operaciones del repositorio de MovimientosInventario.
/// </summary>
public interface IMovimientoInventarioRepository
{
    Task<IReadOnlyList<MovimientoInventario>> GetByProductoIdAsync(Guid productoId, CancellationToken cancellationToken = default);
    Task<MovimientoInventario?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(MovimientoInventario movimiento, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
