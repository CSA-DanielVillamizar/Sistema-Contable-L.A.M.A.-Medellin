using LAMAMedellin.Application.Features.Transacciones.Commands.RegistrarIngreso;
using LAMAMedellin.Application.Features.Transacciones.Commands.RegistrarEgreso;
using LAMAMedellin.Application.Features.Transacciones.Queries.GetCatalogoBancos;
using LAMAMedellin.Application.Features.Transacciones.Queries.GetCatalogoCentrosCosto;
using LAMAMedellin.Application.Features.Transacciones.Queries.GetTransacciones;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/transacciones")]
[Authorize]
public sealed class TransaccionesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<TransaccionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransacciones(CancellationToken cancellationToken)
    {
        var transacciones = await sender.Send(new GetTransaccionesQuery(), cancellationToken);
        return Ok(transacciones);
    }

    [HttpGet("bancos")]
    [ProducesResponseType(typeof(List<CatalogoBancoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalogoBancos(CancellationToken cancellationToken)
    {
        var bancos = await sender.Send(new GetCatalogoBancosQuery(), cancellationToken);
        return Ok(bancos);
    }

    [HttpGet("centros-costo")]
    [ProducesResponseType(typeof(List<CatalogoCentroCostoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalogoCentrosCosto(CancellationToken cancellationToken)
    {
        var centrosCosto = await sender.Send(new GetCatalogoCentrosCostoQuery(), cancellationToken);
        return Ok(centrosCosto);
    }

    [HttpPost("ingreso")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> RegistrarIngreso([FromBody] RegistrarIngresoCommand command, CancellationToken cancellationToken)
    {
        var transaccionId = await sender.Send(command, cancellationToken);

        return Created($"/api/transacciones/{transaccionId}", new { id = transaccionId });
    }

    [HttpPost("egreso")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> RegistrarEgreso([FromBody] RegistrarEgresoCommand command, CancellationToken cancellationToken)
    {
        var transaccionId = await sender.Send(command, cancellationToken);

        return Created($"/api/transacciones/{transaccionId}", new { id = transaccionId });
    }
}
