using LAMAMedellin.Application.Features.Tesoreria.Commands.RegistrarEgreso;
using LAMAMedellin.Application.Features.Tesoreria.Commands.RegistrarIngreso;
using LAMAMedellin.Application.Features.Tesoreria.Queries.GetCuentasBancarias;
using LAMAMedellin.Application.Features.Tesoreria.Queries.GetEgresos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/tesoreria")]
[Authorize(Roles = "Admin,Tesorero")]
public sealed class TesoreriaController(ISender sender) : ControllerBase
{
    [HttpGet("cuentas-bancarias")]
    [ProducesResponseType(typeof(List<CuentaBancariaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCuentasBancarias(CancellationToken cancellationToken)
    {
        var cuentas = await sender.Send(new GetCuentasBancariasQuery(), cancellationToken);
        return Ok(cuentas);
    }

    [HttpGet("egresos")]
    [ProducesResponseType(typeof(List<EgresoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEgresos(CancellationToken cancellationToken)
    {
        var egresos = await sender.Send(new GetEgresosQuery(), cancellationToken);
        return Ok(egresos);
    }

    [HttpPost("egresos")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> RegistrarEgreso([FromBody] RegistrarEgresoCommand command, CancellationToken cancellationToken)
    {
        var egresoId = await sender.Send(command, cancellationToken);
        return Created($"/api/tesoreria/egresos/{egresoId}", new { id = egresoId });
    }

    [HttpPost("ingresos")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> RegistrarIngreso([FromBody] RegistrarIngresoCommand command, CancellationToken cancellationToken)
    {
        var ingresoId = await sender.Send(command, cancellationToken);
        return Created($"/api/tesoreria/ingresos/{ingresoId}", new { id = ingresoId });
    }
}
