using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface ISolicitudAnulacionRepository
{
    Task<List<SolicitudAnulacion>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<SolicitudAnulacion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Evita dos solicitudes abiertas sobre el mismo comprobante: dos personas
    /// aprobando en paralelo lo anularian dos veces.
    /// </summary>
    Task<bool> ExistePendienteAsync(Guid comprobanteId, CancellationToken cancellationToken = default);

    Task AddAsync(SolicitudAnulacion solicitud, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
