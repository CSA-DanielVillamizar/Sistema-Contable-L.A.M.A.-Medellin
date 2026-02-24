using LAMAMedellin.Application.Features.Cartera.Commands.GenerarCarteraMensual;
using LAMAMedellin.Application.Features.Cartera.Commands.RegistrarPagoCartera;
using LAMAMedellin.Application.Features.Cartera.Queries.GetCarteraPendiente;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/cartera")]
[Authorize]
public sealed class CarteraController(ISender sender) : ControllerBase
{
    [HttpPost("generar-mensual")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerarCarteraMensual(
        [FromBody] GenerarCarteraMensualRequest request,
        CancellationToken cancellationToken)
    {
        var resultado = await sender.Send(new GenerarCarteraMensualCommand(request.Periodo), cancellationToken);

        return Ok(new
        {
            mensaje = $"Se han generado {resultado} obligaciones para el periodo {request.Periodo}"
        });
    }

    [HttpGet("pendiente")]
    [ProducesResponseType(typeof(List<CarteraPendienteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCarteraPendiente(CancellationToken cancellationToken)
    {
        var cartera = await sender.Send(new GetCarteraPendienteQuery(), cancellationToken);
        return Ok(cartera);
    }

    [HttpPost("{id:guid}/pago")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegistrarPago(
        Guid id,
        [FromBody] RegistrarPagoRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new RegistrarPagoCarteraCommand(
            id,
            request.MontoPagadoCOP,
            request.BancoId,
            request.CentroCostoId,
            request.Descripcion), cancellationToken);

        return Ok(new
        {
            mensaje = "Pago registrado correctamente."
        });
    }
}

public sealed record GenerarCarteraMensualRequest(string Periodo);
public sealed record RegistrarPagoRequest(decimal MontoPagadoCOP, Guid BancoId, Guid CentroCostoId, string? Descripcion);
