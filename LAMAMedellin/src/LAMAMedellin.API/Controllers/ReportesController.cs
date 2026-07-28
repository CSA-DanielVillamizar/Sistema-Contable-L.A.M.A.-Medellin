using LAMAMedellin.Application.Features.Contabilidad.Queries.GetBalancePrueba;
using LAMAMedellin.Application.Features.Contabilidad.Queries.GetLibroDiario;
using LAMAMedellin.Application.Features.Contabilidad.Queries.GetLibroMayor;
using LAMAMedellin.Application.Features.Reportes.Queries.GetCarteraMora;
using LAMAMedellin.Application.Features.Reportes.Queries.GetEstadoResultados;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/reportes")]
[Authorize]
public sealed class ReportesController(ISender sender) : ControllerBase
{
    // ------------------------------------------------------------------
    // Libros oficiales (historia 1-4).
    //
    // Solo lectura y restringidos: el Contador y la Junta los consultan, y el
    // Tesorero los necesita para poder validar el mes antes del cierre.
    // Leen unicamente comprobantes asentados.
    // ------------------------------------------------------------------

    [HttpGet("libro-diario")]
    [Authorize(Roles = "Contador,Admin,Junta,Tesorero")]
    [ProducesResponseType(typeof(LibroDiarioDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLibroDiario(
        [FromQuery] DateOnly desde,
        [FromQuery] DateOnly hasta,
        [FromQuery] Guid? centroCostoId,
        CancellationToken cancellationToken)
    {
        var libro = await sender.Send(new GetLibroDiarioQuery(desde, hasta, centroCostoId), cancellationToken);
        return Ok(libro);
    }

    [HttpGet("libro-mayor")]
    [Authorize(Roles = "Contador,Admin,Junta,Tesorero")]
    [ProducesResponseType(typeof(LibroMayorDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLibroMayor(
        [FromQuery] Guid cuentaContableId,
        [FromQuery] DateOnly desde,
        [FromQuery] DateOnly hasta,
        [FromQuery] Guid? centroCostoId,
        CancellationToken cancellationToken)
    {
        var libro = await sender.Send(
            new GetLibroMayorQuery(cuentaContableId, desde, hasta, centroCostoId),
            cancellationToken);

        return Ok(libro);
    }

    [HttpGet("balance-prueba")]
    [Authorize(Roles = "Contador,Admin,Junta,Tesorero")]
    [ProducesResponseType(typeof(BalancePruebaDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalancePrueba(
        [FromQuery] int anio,
        [FromQuery] int mes,
        [FromQuery] Guid? centroCostoId,
        CancellationToken cancellationToken)
    {
        var balance = await sender.Send(new GetBalancePruebaQuery(anio, mes, centroCostoId), cancellationToken);
        return Ok(balance);
    }

    [HttpGet("estado-resultados")]
    [ProducesResponseType(typeof(EstadoResultadosDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEstadoResultados(
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin,
        CancellationToken cancellationToken)
    {
        var reporte = await sender.Send(
            new GetEstadoResultadosQuery(fechaInicio, fechaFin),
            cancellationToken);

        return Ok(reporte);
    }

    [HttpGet("cartera-mora")]
    [ProducesResponseType(typeof(CarteraMoraDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCarteraMora(CancellationToken cancellationToken)
    {
        var reporte = await sender.Send(new GetCarteraMoraQuery(), cancellationToken);
        return Ok(reporte);
    }
}
