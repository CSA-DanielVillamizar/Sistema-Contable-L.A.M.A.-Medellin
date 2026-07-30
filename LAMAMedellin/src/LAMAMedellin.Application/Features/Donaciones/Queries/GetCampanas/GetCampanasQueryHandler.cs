using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Queries.GetCampanas;

public sealed class GetCampanasQueryHandler(
    ICampanaDonacionRepository campanaRepository,
    IDonacionRepository donacionRepository)
    : IRequestHandler<GetCampanasQuery, IReadOnlyList<CampanaDonacionDto>>
{
    public async Task<IReadOnlyList<CampanaDonacionDto>> Handle(
        GetCampanasQuery request,
        CancellationToken cancellationToken)
    {
        var campanas = await campanaRepository.GetAllAsync(cancellationToken);
        var donaciones = await donacionRepository.GetAllWithDetallesAsync(cancellationToken);

        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

        return campanas
            .Where(c => request.IncluirCerradas || c.EstaActiva)
            .OrderByDescending(c => c.FechaInicio)
            .Select(c =>
            {
                var propias = donaciones.Where(d => d.CampanaDonacionId == c.Id).ToList();
                var recaudado = propias.Sum(d => d.MontoCOP);

                return new CampanaDonacionDto(
                    c.Id,
                    c.Nombre,
                    c.Descripcion,
                    c.MetaCOP,
                    recaudado,
                    c.MetaCOP == 0 ? 0 : decimal.Round(recaudado / c.MetaCOP * 100m, 1),
                    propias.Count,
                    c.FechaInicio,
                    c.FechaFin,
                    c.EstaActiva,
                    c.EstaActiva && hoy >= c.FechaInicio && hoy <= c.FechaFin);
            })
            .ToList();
    }
}
