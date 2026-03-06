using LAMAMedellin.Application.Features.Tributario.Queries.GetReporteBeneficiariosFinales;
using LAMAMedellin.Application.Features.Tributario.Queries.GetReporteCalidadDatos;
using LAMAMedellin.Application.Features.Tributario.Queries.GetReporteExogena;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/tributario")]
[Authorize(Roles = "Contador,Admin")]
public sealed class TributarioController(ISender sender) : ControllerBase
{
    [HttpGet("calidad-datos")]
    [ProducesResponseType(typeof(IReadOnlyList<InconsistenciaTributariaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReporteCalidadDatos(CancellationToken cancellationToken)
    {
        var reporte = await sender.Send(new GetReporteCalidadDatosQuery(), cancellationToken);
        return Ok(reporte);
    }

    [HttpGet("beneficiarios-finales")]
    [ProducesResponseType(typeof(IReadOnlyList<BeneficiarioFinalDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReporteBeneficiariosFinales(CancellationToken cancellationToken)
    {
        var reporte = await sender.Send(new GetReporteBeneficiariosFinalesQuery(), cancellationToken);
        return Ok(reporte);
    }

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
