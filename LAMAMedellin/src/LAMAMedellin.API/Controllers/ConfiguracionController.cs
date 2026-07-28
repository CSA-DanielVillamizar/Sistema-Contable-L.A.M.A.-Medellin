using LAMAMedellin.Application.Features.Configuracion.CuotasAsamblea.Commands.ActualizarRenovacionMembresia;
using LAMAMedellin.Application.Features.Configuracion.Tarifas.Commands.ActualizarTarifasCuota;
using LAMAMedellin.Application.Features.Configuracion.Tarifas.Queries.GetTarifasCuota;
using LAMAMedellin.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/configuracion")]
[Authorize(Roles = "Admin")]
public sealed class ConfiguracionController(ISender sender) : ControllerBase
{
    [HttpGet("tarifas")]
    [ProducesResponseType(typeof(List<TarifaCuotaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTarifas(CancellationToken cancellationToken)
    {
        var tarifas = await sender.Send(new GetTarifasCuotaQuery(), cancellationToken);

        var response = tarifas
            .Select(x => new TarifaCuotaResponse(
                x.TipoAfiliacion,
                x.ValorMensualCOP))
            .ToList();

        return Ok(response);
    }

    [HttpPut("tarifas")]
    [ProducesResponseType(typeof(List<TarifaCuotaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutTarifas(
        [FromBody] ActualizarTarifasRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ActualizarTarifasCuotaCommand(
            request.Tarifas
                .Select(x => new ActualizarTarifaCuotaItem(x.TipoAfiliacion, x.ValorMensualCOP))
                .ToList());

        var tarifas = await sender.Send(command, cancellationToken);

        var response = tarifas
            .Select(x => new TarifaCuotaResponse(
                x.TipoAfiliacion,
                x.ValorMensualCOP))
            .ToList();

        return Ok(response);
    }

    [HttpPatch("cuotas-asamblea/{anio:int}/renovacion-membresia")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> PatchRenovacionMembresia(
        int anio,
        [FromBody] ActualizarRenovacionMembresiaRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new ActualizarRenovacionMembresiaCommand(anio, request.RenovacionMembresiaUSD),
            cancellationToken);

        return NoContent();
    }
}

public sealed record ActualizarTarifasRequest(List<TarifaCuotaRequest> Tarifas);
public sealed record TarifaCuotaRequest(TipoAfiliacion TipoAfiliacion, decimal ValorMensualCOP);
public sealed record TarifaCuotaResponse(TipoAfiliacion TipoAfiliacion, decimal ValorMensualCOP);
public sealed record ActualizarRenovacionMembresiaRequest(decimal? RenovacionMembresiaUSD);
