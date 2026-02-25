using LAMAMedellin.Application.Features.Proyectos.Commands.CreateProyectoSocial;
using LAMAMedellin.Application.Features.Proyectos.Commands.DeleteProyectoSocial;
using LAMAMedellin.Application.Features.Proyectos.Commands.UpdateProyectoSocial;
using LAMAMedellin.Application.Features.Proyectos.Queries.GetProyectoSocialById;
using LAMAMedellin.Application.Features.Proyectos.Queries.GetProyectosSociales;
using LAMAMedellin.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/proyectos")]
[Authorize]
public sealed class ProyectosController(ISender sender) : ControllerBase
{
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
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] UpsertProyectoRequest request, CancellationToken cancellationToken)
    {
        var id = await sender.Send(
            new CreateProyectoSocialCommand(
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpsertProyectoRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateProyectoSocialCommand(
                id,
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteProyectoSocialCommand(id), cancellationToken);
        return NoContent();
    }

    public sealed record UpsertProyectoRequest(
        string Nombre,
        string Descripcion,
        DateTime FechaInicio,
        DateTime? FechaFin,
        decimal PresupuestoEstimado,
        EstadoProyectoSocial Estado);
}
