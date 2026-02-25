using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface IProyectoSocialRepository
{
    Task<IReadOnlyList<ProyectoSocial>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProyectoSocial?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ProyectoSocial proyectoSocial, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
