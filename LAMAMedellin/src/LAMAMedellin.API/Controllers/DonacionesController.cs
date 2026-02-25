using LAMAMedellin.Application.Features.Donaciones.Commands.CrearDonante;
using LAMAMedellin.Application.Features.Donaciones.Commands.RegistrarDonacion;
using LAMAMedellin.Application.Features.Donaciones.Queries.GetDonaciones;
using LAMAMedellin.Application.Features.Donaciones.Queries.GetDonantes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/donaciones")]
[Authorize]
public sealed class DonacionesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<DonacionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDonaciones(CancellationToken cancellationToken)
    {
        var donaciones = await sender.Send(new GetDonacionesQuery(), cancellationToken);
        return Ok(donaciones);
    }

    [HttpGet("donantes")]
    [ProducesResponseType(typeof(List<DonanteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDonantes(CancellationToken cancellationToken)
    {
        var donantes = await sender.Send(new GetDonantesQuery(), cancellationToken);
        return Ok(donantes);
    }

    [HttpPost("donantes")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> CrearDonante([FromBody] CrearDonanteCommand command, CancellationToken cancellationToken)
    {
        var donanteId = await sender.Send(command, cancellationToken);
        return Created($"/api/donaciones/donantes/{donanteId}", new { id = donanteId });
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> RegistrarDonacion([FromBody] RegistrarDonacionCommand command, CancellationToken cancellationToken)
    {
        var donacionId = await sender.Send(command, cancellationToken);
        return Created($"/api/donaciones/{donacionId}", new { id = donacionId });
    }
}
