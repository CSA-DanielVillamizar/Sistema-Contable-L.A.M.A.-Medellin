using MediatR;

namespace LAMAMedellin.Application.Features.Proyectos.Queries.GetRendicionProyecto;

/// <summary>
/// Indicadores e informe de rendicion (historia 3-4). Sin proyecto indicado
/// devuelve el consolidado de todos.
/// </summary>
public sealed record GetRendicionProyectoQuery(Guid? ProyectoSocialId = null)
    : IRequest<IReadOnlyList<RendicionProyectoDto>>;

/// <summary>
/// Lo que se rinde de un proyecto.
///
/// Lo ejecutado sale de los asientos imputados al centro de costo del
/// proyecto, no de un campo que alguien actualice a mano: es la unica cifra que
/// el libro respalda y la unica que un auditor puede rastrear.
///
/// No lleva datos de beneficiarios, solo su conteo: la rendicion es publica por
/// naturaleza y la PII esta restringida por rol (historia 3-3).
/// </summary>
public sealed record RendicionProyectoDto(
    Guid ProyectoSocialId,
    string Nombre,
    string Estado,
    DateTime FechaInicio,
    DateTime? FechaFin,
    decimal PresupuestoEstimado,
    decimal PresupuestoAsignadoAActividades,
    decimal EjecutadoCOP,
    decimal DisponibleCOP,
    decimal PorcentajeEjecucion,
    int TotalActividades,
    int ActividadesCompletadas,
    int ActividadesVencidas,
    decimal PorcentajeAvanceActividades,
    int TotalBeneficiarios);
