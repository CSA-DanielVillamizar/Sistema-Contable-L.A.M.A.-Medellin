using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface IDonanteRepository
{
    Task<IReadOnlyList<Donante>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Donante?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Donante?> GetByDocumentoAsync(TipoDocumentoDonante tipoDocumento, string numeroDocumento, CancellationToken cancellationToken = default);
    Task AddAsync(Donante donante, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
