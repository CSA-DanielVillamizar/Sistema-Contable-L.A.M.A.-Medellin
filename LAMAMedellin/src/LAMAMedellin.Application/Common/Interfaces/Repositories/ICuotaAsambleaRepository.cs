using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface ICuotaAsambleaRepository
{
    Task<CuotaAsamblea?> GetVigentePorPeriodoAsync(int anio, int mes, CancellationToken cancellationToken = default);
}
