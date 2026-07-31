using LAMAMedellin.Application.Features.Proyectos.Commands.CreateProyectoSocial;
using LAMAMedellin.Application.Features.Proyectos.Commands.DeleteProyectoSocial;
using LAMAMedellin.Application.Features.Proyectos.Commands.UpdateProyectoSocial;
using LAMAMedellin.Application.Features.Proyectos.Queries.GetProyectoSocialById;
using LAMAMedellin.Application.Features.Proyectos.Queries.GetProyectosSociales;
using LAMAMedellin.Domain.Enums;
using LAMAMedellin.Application.Features.Proyectos.Commands.ActualizarActividad;
using LAMAMedellin.Application.Features.Proyectos.Commands.CambiarEstadoActividad;
using LAMAMedellin.Application.Features.Proyectos.Commands.CrearActividad;
using LAMAMedellin.Application.Features.Proyectos.Queries.GetActividades;
using LAMAMedellin.Application.Features.Proyectos.Queries.GetRendicionProyecto;
using MediatR;
using LAMAMedellin.API.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/proyectos")][Authorize(Roles = Roles.ProyectosLectura)]
public sealed class ProyectosController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Indicadores e informe de rendicion (historia 3-4). Sin id devuelve el
    /// consolidado de todos los proyectos.
    /// </summary>
    [HttpGet("rendicion")]
    [ProducesResponseType(typeof(IReadOnlyList<RendicionProyectoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRendicion(
        [FromQuery] Guid? proyectoSocialId,
        CancellationToken cancellationToken)
    {
        var rendicion = await sender.Send(new GetRendicionProyectoQuery(proyectoSocialId), cancellationToken);
        return Ok(rendicion);
    }

    [HttpGet("{id:guid}/actividades")]
    [ProducesResponseType(typeof(IReadOnlyList<ActividadProyectoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActividades(Guid id, CancellationToken cancellationToken)
    {
        var actividades = await sender.Send(new GetActividadesQuery(id), cancellationToken);
        return Ok(actividades);
    }

    [HttpPost("{id:guid}/actividades")]
    [Authorize(Roles = Roles.ProyectosEscritura)]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> CrearActividad(
        Guid id,
        [FromBody] CrearActividadRequest request,
        CancellationToken cancellationToken)
    {
        var actividadId = await sender.Send(
            new CrearActividadCommand(id, request.Nombre, request.Descripcion,
                request.FechaInicioPlanificada, request.FechaFinPlanificada,
                request.PresupuestoAsignado, request.Responsable),
            cancellationToken);

        return Created($"/api/proyectos/{id}/actividades/{actividadId}", new { id = actividadId });
    }

    [HttpPut("actividades/{actividadId:guid}")]
    [Authorize(Roles = Roles.ProyectosEscritura)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ActualizarActividad(
        Guid actividadId,
        [FromBody] CrearActividadRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new ActualizarActividadCommand(actividadId, request.Nombre, request.Descripcion,
                request.FechaInicioPlanificada, request.FechaFinPlanificada,
                request.PresupuestoAsignado, request.Responsable),
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("actividades/{actividadId:guid}/estado")]
    [Authorize(Roles = Roles.ProyectosEscritura)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CambiarEstadoActividad(
        Guid actividadId,
        [FromBody] CambiarEstadoActividadRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new CambiarEstadoActividadCommand(actividadId, request.Estado), cancellationToken);
        return NoContent();
    }

    public sealed record CrearActividadRequest(
        string Nombre,
        string Descripcion,
        DateOnly FechaInicioPlanificada,
        DateOnly FechaFinPlanificada,
        decimal PresupuestoAsignado,
        string? Responsable);

    public sealed record CambiarEstadoActividadRequest(EstadoActividadProyecto Estado);

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LAMAMedellin.Application.Features.Proyectos.Queries.GetProyectosSociales.ProyectoSocialDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var proyectos = await sender.Send(new GetProyectosSocialesQuery(), cancellationToken);
        return Ok(proyectos);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LAMAMedellin.Application.Features.Proyectos.Queries.GetProyectoSocialById.ProyectoSocialDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var proyecto = await sender.Send(new GetProyectoSocialByIdQuery(id), cancellationToken);
        if (proyecto is null)
        {
            return NotFound();
        }

        return Ok(proyecto);
    }

    [HttpPost]
    [Authorize(Roles = Roles.ProyectosEscritura)]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] UpsertProyectoRequest request, CancellationToken cancellationToken)
    {
        var id = await sender.Send(
            new CreateProyectoSocialCommand(
                request.CentroCostoId,
                request.Nombre,
                request.Descripcion,
                request.FechaInicio,
                request.FechaFin,
                request.PresupuestoEstimado,
                request.Estado),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.ProyectosEscritura)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpsertProyectoRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateProyectoSocialCommand(
                id,
                request.CentroCostoId,
                request.Nombre,
                request.Descripcion,
                request.FechaInicio,
                request.FechaFin,
                request.PresupuestoEstimado,
                request.Estado),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.ProyectosEscritura)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteProyectoSocialCommand(id), cancellationToken);
        return NoContent();
    }

    public sealed record UpsertProyectoRequest(
        Guid CentroCostoId,
        string Nombre,
        string Descripcion,
        DateTime FechaInicio,
        DateTime? FechaFin,
        decimal PresupuestoEstimado,
        EstadoProyectoSocial Estado);
}
