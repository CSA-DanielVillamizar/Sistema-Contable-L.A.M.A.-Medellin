using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface IMapeoContableRepository
{
    Task<List<MapeoContable>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<MapeoContable?> GetPorOperacionAsync(
        TipoOperacionContable tipoOperacion,
        CancellationToken cancellationToken = default);

    Task AddAsync(MapeoContable mapeo, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
