using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface IActividadProyectoRepository
{
    Task<List<ActividadProyecto>> GetPorProyectoAsync(Guid proyectoSocialId, CancellationToken cancellationToken = default);

    Task<List<ActividadProyecto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ActividadProyecto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(ActividadProyecto actividad, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
