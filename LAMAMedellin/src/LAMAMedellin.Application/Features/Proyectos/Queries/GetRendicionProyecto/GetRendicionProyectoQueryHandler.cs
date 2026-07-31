using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Proyectos.Queries.GetRendicionProyecto;

public sealed class GetRendicionProyectoQueryHandler(
    IProyectoSocialRepository proyectoRepository,
    IActividadProyectoRepository actividadRepository,
    IBeneficiarioRepository beneficiarioRepository,
    ILibrosContablesRepository librosRepository)
    : IRequestHandler<GetRendicionProyectoQuery, IReadOnlyList<RendicionProyectoDto>>
{
    public async Task<IReadOnlyList<RendicionProyectoDto>> Handle(
        GetRendicionProyectoQuery request,
        CancellationToken cancellationToken)
    {
        var proyectos = await proyectoRepository.GetAllAsync(cancellationToken);
        var actividades = await actividadRepository.GetAllAsync(cancellationToken);
        var beneficiarios = await beneficiarioRepository.GetAllAsync(cancellationToken);

        var seleccionados = request.ProyectoSocialId is null
            ? proyectos
            : proyectos.Where(p => p.Id == request.ProyectoSocialId).ToList();

        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        var resultado = new List<RendicionProyectoDto>(seleccionados.Count);

        foreach (var proyecto in seleccionados)
        {
            // Lo ejecutado sale del libro, no de un campo que alguien mantenga
            // a mano: es la unica cifra que un auditor puede rastrear hasta su
            // comprobante.
            var ejecutado = await librosRepository.GetEjecutadoPorCentroCostoAsync(
                proyecto.CentroCostoId,
                cancellationToken);

            var propias = actividades.Where(a => a.ProyectoSocialId == proyecto.Id).ToList();

            var completadas = propias.Count(a => a.Estado == EstadoActividadProyecto.Completada);
            var vencidas = propias.Count(a =>
                a.FechaFinPlanificada < hoy
                && a.Estado is not EstadoActividadProyecto.Completada
                && a.Estado is not EstadoActividadProyecto.Cancelada);

            // Las canceladas no cuentan para el avance: exigir completar algo
            // que se decidio no hacer dejaria el proyecto siempre incompleto.
            var contablesParaAvance = propias.Count(a => a.Estado != EstadoActividadProyecto.Cancelada);

            resultado.Add(new RendicionProyectoDto(
                proyecto.Id,
                proyecto.Nombre,
                proyecto.Estado.ToString(),
                proyecto.FechaInicio,
                proyecto.FechaFin,
                proyecto.PresupuestoEstimado,
                propias.Sum(a => a.PresupuestoAsignado),
                ejecutado,
                proyecto.PresupuestoEstimado - ejecutado,
                proyecto.PresupuestoEstimado == 0
                    ? 0m
                    : decimal.Round(ejecutado / proyecto.PresupuestoEstimado * 100m, 1),
                propias.Count,
                completadas,
                vencidas,
                contablesParaAvance == 0
                    ? 0m
                    : decimal.Round((decimal)completadas / contablesParaAvance * 100m, 1),
                // Solo el conteo: la rendicion es publica por naturaleza y la
                // PII de beneficiarios esta restringida por rol (historia 3-3).
                beneficiarios.Count(b => b.ProyectoSocialId == proyecto.Id)));
        }

        return resultado.OrderBy(r => r.Nombre).ToList();
    }
}
