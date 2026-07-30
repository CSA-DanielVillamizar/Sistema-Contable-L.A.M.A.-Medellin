using LAMAMedellin.API.Authorization;
using LAMAMedellin.Application.Features.Anulaciones.Commands.ResolverAnulacion;
using LAMAMedellin.Application.Features.Anulaciones.Commands.SolicitarAnulacion;
using LAMAMedellin.Application.Features.Anulaciones.Queries.GetSolicitudesAnulacion;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

/// <summary>
/// Anulacion de comprobantes con aprobacion (historia 1-8).
///
/// La matriz del BRD separa los dos papeles: el Operador solicita y el Tesorero
/// aprueba. Por eso resolver exige un rol distinto del de solicitar; que la
/// misma persona hiciera ambas cosas anularia el control.
/// </summary>
[ApiController]
[Route("api/anulaciones")]
[Authorize(Roles = Roles.TesoreriaLectura)]
public sealed class AnulacionesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SolicitudAnulacionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var solicitudes = await sender.Send(new GetSolicitudesAnulacionQuery(), cancellationToken);
        return Ok(solicitudes);
    }

    [HttpPost]
    [Authorize(Roles = Roles.TesoreriaEscritura)]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Solicitar(
        [FromBody] SolicitarAnulacionCommand command,
        CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return Created($"/api/anulaciones/{id}", new { id });
    }

    /// <summary>
    /// Aprobar o rechazar. Solo Tesorero, Contador o Admin: es el control que
    /// justifica el flujo entero.
    /// </summary>
    [HttpPost("{id:guid}/resolver")]
    [Authorize(Roles = Roles.CierreValidar)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Resolver(
        Guid id,
        [FromBody] ResolverAnulacionRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new ResolverAnulacionCommand(id, request.Aprobar, request.Motivo), cancellationToken);
        return NoContent();
    }

    public sealed record ResolverAnulacionRequest(bool Aprobar, string? Motivo);
}
