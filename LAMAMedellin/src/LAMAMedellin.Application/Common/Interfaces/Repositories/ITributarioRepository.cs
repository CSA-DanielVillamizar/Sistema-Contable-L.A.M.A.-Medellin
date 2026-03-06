using LAMAMedellin.Application.Features.Tributario.Queries.GetReporteExogena;
using LAMAMedellin.Application.Features.Tributario.Queries.GetReporteBeneficiariosFinales;
using LAMAMedellin.Application.Features.Tributario.Queries.GetReporteCalidadDatos;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface ITributarioRepository
{
    Task<IReadOnlyList<ReporteExogenaDto>> GetReporteExogenaAsync(int anio, int? mes, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BeneficiarioFinalDto>> GetReporteBeneficiariosFinalesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InconsistenciaTributariaDto>> GetReporteCalidadDatosAsync(CancellationToken cancellationToken = default);
}
