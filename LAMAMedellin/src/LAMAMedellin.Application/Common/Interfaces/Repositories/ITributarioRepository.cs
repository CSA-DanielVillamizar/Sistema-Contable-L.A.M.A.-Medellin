using LAMAMedellin.Application.Features.Tributario.Queries.GetReporteExogena;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface ITributarioRepository
{
    Task<IReadOnlyList<ReporteExogenaDto>> GetReporteExogenaAsync(int anio, int? mes, CancellationToken cancellationToken = default);
}
