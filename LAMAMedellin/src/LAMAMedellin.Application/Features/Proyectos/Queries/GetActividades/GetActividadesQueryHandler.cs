using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Proyectos.Queries.GetActividades;

public sealed class GetActividadesQueryHandler(IActividadProyectoRepository actividadRepository)
    : IRequestHandler<GetActividadesQuery, IReadOnlyList<ActividadProyectoDto>>
{
    public async Task<IReadOnlyList<ActividadProyectoDto>> Handle(
        GetActividadesQuery request,
        CancellationToken cancellationToken)
    {
        var actividades = await actividadRepository.GetPorProyectoAsync(request.ProyectoSocialId, cancellationToken);
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

        return actividades
            .OrderBy(a => a.FechaInicioPlanificada)
            .Select(a => new ActividadProyectoDto(
                a.Id,
                a.ProyectoSocialId,
                a.Nombre,
                a.Descripcion,
                a.FechaInicioPlanificada,
                a.FechaFinPlanificada,
                a.PresupuestoAsignado,
                (int)a.Estado,
                a.Estado.ToString(),
                a.Responsable,
                // Vencida es la que paso su fecha sin completarse. Es la senal
                // que el coordinador necesita, no la fecha en si.
                a.FechaFinPlanificada < hoy
                    && a.Estado is not EstadoActividadProyecto.Completada
                    && a.Estado is not EstadoActividadProyecto.Cancelada))
            .ToList();
    }
}
