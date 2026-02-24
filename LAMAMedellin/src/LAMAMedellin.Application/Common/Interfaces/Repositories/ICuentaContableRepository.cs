using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface ICuentaContableRepository
{
    Task<List<CuentaContable>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<CuentaContable>> GetAsentablesAsync(CancellationToken cancellationToken = default);
    Task<CuentaContable?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default);
    Task<CuentaContable?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
