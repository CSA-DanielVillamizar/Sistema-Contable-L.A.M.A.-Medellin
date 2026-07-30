using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface ICuentaPorPagarRepository
{
    Task<List<CuentaPorPagar>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<CuentaPorPagar?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Una misma factura de un mismo proveedor no puede registrarse dos veces:
    /// duplicaria el pasivo y el gasto.
    /// </summary>
    Task<bool> ExisteFacturaAsync(
        string nitProveedor,
        string numeroFactura,
        CancellationToken cancellationToken = default);

    Task AddAsync(CuentaPorPagar cuenta, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
