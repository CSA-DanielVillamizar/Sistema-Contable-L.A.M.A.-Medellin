using LAMAMedellin.Application.Features.Contabilidad.Commands.CerrarPeriodo;
using LAMAMedellin.Application.Features.Contabilidad.Commands.ValidarPeriodoTesoreria;
using LAMAMedellin.Application.Features.Contabilidad.Queries.GetPeriodosContables;
using MediatR;
using LAMAMedellin.API.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

/// <summary>
/// Cierre contable mensual. La separacion de roles entre validar y cerrar es
/// deliberada: es el control de segregacion de funciones que pide el backlog,
/// no un detalle de permisos.
/// </summary>
[ApiController]
[Route("api/periodos-contables")][Authorize(Roles = Roles.CierreLectura)]
public sealed class PeriodosContablesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PeriodoContableDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var periodos = await sender.Send(new GetPeriodosContablesQuery(), cancellationToken);
        return Ok(periodos);
    }

    /// <summary>Paso 1: el Tesorero da por revisado el mes.</summary>
    [HttpPost("{anio:int}/{mes:int}/validar")]
    [Authorize(Roles = Roles.CierreValidar)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Validar(int anio, int mes, CancellationToken cancellationToken)
    {
        await sender.Send(new ValidarPeriodoTesoreriaCommand(anio, mes), cancellationToken);

        return Ok(new { mensaje = $"Periodo {anio}-{mes:D2} validado por tesoreria." });
    }

    /// <summary>Paso 2: el Contador cierra y el periodo queda bloqueado.</summary>
    [HttpPost("{anio:int}/{mes:int}/cerrar")]
    [Authorize(Roles = Roles.CierreEjecutar)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Cerrar(int anio, int mes, CancellationToken cancellationToken)
    {
        await sender.Send(new CerrarPeriodoCommand(anio, mes), cancellationToken);

        return Ok(new { mensaje = $"Periodo {anio}-{mes:D2} cerrado." });
    }
}
