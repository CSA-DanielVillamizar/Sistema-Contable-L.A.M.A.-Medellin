using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface IPeriodoContableRepository
{
    Task<IReadOnlyList<PeriodoContable>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PeriodoContable?> GetPorAnioYMesAsync(int anio, int mes, CancellationToken cancellationToken = default);

    /// <summary>Comprobantes en Borrador dentro del periodo: impiden cerrarlo.</summary>
    Task<int> ContarComprobantesEnBorradorAsync(int anio, int mes, CancellationToken cancellationToken = default);

    Task AddAsync(PeriodoContable periodo, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
