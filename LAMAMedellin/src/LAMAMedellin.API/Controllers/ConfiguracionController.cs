using LAMAMedellin.Application.Features.Configuracion.Commands.ActualizarCentroCosto;
using LAMAMedellin.Application.Features.Configuracion.Commands.CrearCentroCosto;
using LAMAMedellin.Application.Features.Configuracion.MapeoContable.Commands.ActualizarMapeoContable;
using LAMAMedellin.Application.Features.Configuracion.MapeoContable.Queries.GetMapeosContables;
using LAMAMedellin.Application.Features.Configuracion.CuotasAsamblea.Commands.ActualizarRenovacionMembresia;
using LAMAMedellin.Application.Features.Configuracion.Tarifas.Commands.ActualizarTarifasCuota;
using LAMAMedellin.Application.Features.Configuracion.Tarifas.Queries.GetTarifasCuota;
using LAMAMedellin.Domain.Enums;
using MediatR;
using LAMAMedellin.API.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/configuracion")]
[Authorize(Roles = Roles.ConfiguracionLectura)]
public sealed class ConfiguracionController(ISender sender) : ControllerBase
{
    [HttpPost("centros-costo")]
    [Authorize(Roles = Roles.ConfiguracionEscritura)]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> CrearCentroCosto(
        [FromBody] CrearCentroCostoCommand command,
        CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return Created($"/api/configuracion/centros-costo/{id}", new { id });
    }

    /// <summary>
    /// No hay baja de centros de costo: los asientos ya imputados a uno deben
    /// conservar su imputacion, y borrarlo dejaria huerfano el historico.
    /// </summary>
    [HttpPut("centros-costo/{id:guid}")]
    [Authorize(Roles = Roles.ConfiguracionEscritura)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ActualizarCentroCosto(
        Guid id,
        [FromBody] ActualizarCentroCostoRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new ActualizarCentroCostoCommand(id, request.Nombre, request.Tipo), cancellationToken);
        return NoContent();
    }

    public sealed record ActualizarCentroCostoRequest(string Nombre, TipoCentroCosto Tipo);

    /// <summary>
    /// Que cuenta usa cada operacion (historia 1-2). Devuelve todas las
    /// operaciones, incluidas las que aun no se han configurado.
    /// </summary>
    [HttpGet("mapeo-contable")]
    [ProducesResponseType(typeof(IReadOnlyList<MapeoContableDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMapeoContable(CancellationToken cancellationToken)
    {
        var mapeos = await sender.Send(new GetMapeosContablesQuery(), cancellationToken);
        return Ok(mapeos);
    }

    [HttpPut("mapeo-contable/{tipoOperacion:int}")]
    [Authorize(Roles = Roles.ConfiguracionEscritura)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PutMapeoContable(
        int tipoOperacion,
        [FromBody] ActualizarMapeoContableRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new ActualizarMapeoContableCommand((TipoOperacionContable)tipoOperacion, request.CuentaContableId),
            cancellationToken);

        return NoContent();
    }

    public sealed record ActualizarMapeoContableRequest(Guid CuentaContableId);

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
    [Authorize(Roles = Roles.ConfiguracionEscritura)]
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
    [Authorize(Roles = Roles.ConfiguracionEscritura)]
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
