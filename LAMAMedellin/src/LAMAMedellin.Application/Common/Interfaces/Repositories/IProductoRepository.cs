using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

/// <summary>
/// Interfaz para operaciones del repositorio de Productos.
/// </summary>
public interface IProductoRepository
{
    Task<IReadOnlyList<Producto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Producto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Producto?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task AddAsync(Producto producto, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
