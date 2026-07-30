using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface IComprobanteRepository
{
    Task<Comprobante?> GetByIdWithAsientosAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca por consecutivo. Lo usa la verificacion publica de recibos, que
    /// solo conoce ese numero porque es lo que lleva el QR.
    /// </summary>
    Task<Comprobante?> GetPorConsecutivoAsync(string numeroConsecutivo, CancellationToken cancellationToken = default);
    /// <summary>Los mas recientes primero, con sus asientos para poder totalizar.</summary>
    Task<List<Comprobante>> GetRecientesAsync(int limite, CancellationToken cancellationToken = default);

    Task AddAsync(Comprobante comprobante, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
