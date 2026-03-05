using LAMAMedellin.Application.Features.Tributario.Queries.GetReporteExogena;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/tributario")]
[Authorize]
public sealed class TributarioController(ISender sender) : ControllerBase
{
    [HttpGet("exogena")]
    [ProducesResponseType(typeof(IReadOnlyList<ReporteExogenaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReporteExogena(
        [FromQuery] int anio,
        [FromQuery] int? mes,
        CancellationToken cancellationToken)
    {
        var reporte = await sender.Send(new GetReporteExogenaQuery(anio, mes), cancellationToken);
        return Ok(reporte);
    }
}
