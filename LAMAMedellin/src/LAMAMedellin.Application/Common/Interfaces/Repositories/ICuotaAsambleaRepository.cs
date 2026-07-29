using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface ICuotaAsambleaRepository
{
    Task<CuotaAsamblea?> GetVigentePorPeriodoAsync(int anio, int mes, CancellationToken cancellationToken = default);
    Task<CuotaAsamblea?> GetByAnioAsync(int anio, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
