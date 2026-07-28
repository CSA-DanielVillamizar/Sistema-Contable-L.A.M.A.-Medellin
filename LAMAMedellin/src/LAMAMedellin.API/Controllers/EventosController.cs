using LAMAMedellin.Application.Features.Eventos.Commands.CreateEvento;
using LAMAMedellin.Application.Features.Eventos.Commands.EstablecerCuotaLogistica;
using LAMAMedellin.Application.Features.Eventos.Commands.MarcarAsistencia;
using LAMAMedellin.Application.Features.Eventos.Queries.GetEventoById;
using LAMAMedellin.Application.Features.Eventos.Queries.GetEventos;
using LAMAMedellin.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/eventos")]
[Authorize]
public sealed class EventosController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] CreateEventoRequest request, CancellationToken cancellationToken)
    {
        var id = await sender.Send(
            new CreateEventoCommand(
                request.Nombre,
                request.Descripcion,
                request.FechaProgramada,
                request.LugarEncuentro,
                request.TipoEvento,
                request.Destino,
                request.CuotaLogisticaCOP),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPost("{id:guid}/asistencia")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarcarAsistencia(Guid id, [FromBody] MarcarAsistenciaRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new MarcarAsistenciaCommand(
                id,
                request.MiembroId,
                request.Asistio,
                request.Observaciones),
            cancellationToken);

        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<EventoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var eventos = await sender.Send(new GetEventosQuery(), cancellationToken);
        return Ok(eventos);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventoDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var evento = await sender.Send(new GetEventoByIdQuery(id), cancellationToken);
        if (evento is null)
        {
            return NotFound();
        }

        return Ok(evento);
    }

    [HttpPatch("{id:guid}/cuota-logistica")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> EstablecerCuotaLogistica(Guid id, [FromBody] EstablecerCuotaLogisticaRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new EstablecerCuotaLogisticaCommand(id, request.CuotaLogisticaCOP),
            cancellationToken);

        return NoContent();
    }

    public sealed record CreateEventoRequest(
        string Nombre,
        string Descripcion,
        DateTime FechaProgramada,
        string LugarEncuentro,
        TipoEvento TipoEvento,
        string? Destino,
        decimal? CuotaLogisticaCOP);

    public sealed record MarcarAsistenciaRequest(
        Guid MiembroId,
        bool Asistio,
        string? Observaciones);

    public sealed record EstablecerCuotaLogisticaRequest(decimal? CuotaLogisticaCOP);
}
