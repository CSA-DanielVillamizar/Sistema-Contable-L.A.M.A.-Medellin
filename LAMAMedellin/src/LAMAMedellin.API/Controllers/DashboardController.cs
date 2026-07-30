using LAMAMedellin.Application.Features.Dashboard.Queries.GetResumenCartera;
using LAMAMedellin.Application.Features.Dashboard.Queries.GetResumen;
using LAMAMedellin.Application.Features.Dashboard.Queries.GetSaldosBancos;
using MediatR;
using LAMAMedellin.API.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/dashboard")][Authorize(Roles = Roles.TodosLosRoles)]
public sealed class DashboardController(ISender sender) : ControllerBase
{
    [HttpGet("bancos")]
    [ProducesResponseType(typeof(List<SaldoBancoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSaldosBancos(CancellationToken cancellationToken)
    {
        var bancos = await sender.Send(new GetSaldosBancosQuery(), cancellationToken);
        return Ok(bancos);
    }

    [HttpGet("cartera")]
    [ProducesResponseType(typeof(ResumenCarteraDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetResumenCartera(CancellationToken cancellationToken)
    {
        var resumen = await sender.Send(new GetResumenCarteraQuery(), cancellationToken);
        return Ok(resumen);
    }

    [HttpGet("resumen")]
    [ProducesResponseType(typeof(DashboardResumenDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetResumenDashboard(CancellationToken cancellationToken)
    {
        var resumen = await sender.Send(new GetResumenDashboardQuery(), cancellationToken);
        return Ok(resumen);
    }
}
