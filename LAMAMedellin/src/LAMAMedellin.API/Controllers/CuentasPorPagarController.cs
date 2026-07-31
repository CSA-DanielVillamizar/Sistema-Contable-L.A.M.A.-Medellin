using LAMAMedellin.API.Authorization;
using LAMAMedellin.Application.Features.CuentasPorPagar.Commands.AnularCuentaPorPagar;
using LAMAMedellin.Application.Features.CuentasPorPagar.Commands.PagarCuentaPorPagar;
using LAMAMedellin.Application.Features.CuentasPorPagar.Commands.RegistrarCuentaPorPagar;
using LAMAMedellin.Application.Features.CuentasPorPagar.Queries.GetCuentasPorPagar;
using LAMAMedellin.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

/// <summary>
/// Cuentas por pagar a proveedores (historias 1-13 y 1-14).
///
/// Comparte fila con CxC y tesoreria en la matriz del BRD: Operador y Tesorero
/// registran y pagan; Admin, Contador y Junta consultan.
/// </summary>
[ApiController]
[Route("api/cuentas-por-pagar")]
[Authorize(Roles = Roles.TesoreriaLectura)]
public sealed class CuentasPorPagarController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CuentaPorPagarDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] bool incluirAnuladas,
        CancellationToken cancellationToken)
    {
        var cuentas = await sender.Send(new GetCuentasPorPagarQuery(incluirAnuladas), cancellationToken);
        return Ok(cuentas);
    }

    [HttpPost]
    [Authorize(Roles = Roles.TesoreriaEscritura)]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Registrar(
        [FromBody] RegistrarCuentaPorPagarCommand command,
        CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return Created($"/api/cuentas-por-pagar/{id}", new { id });
    }

    [HttpPost("{id:guid}/pagos")]
    [Authorize(Roles = Roles.TesoreriaEscritura)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Pagar(
        Guid id,
        [FromBody] PagarCuentaPorPagarRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new PagarCuentaPorPagarCommand(id, request.Monto, request.BancoId, request.MedioPago),
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Baja logica de la obligacion. Solo se admite sin pagos aplicados: una
    /// factura con abonos ya movio dinero y anularla los dejaria sin explicar.
    /// </summary>
    [HttpPost("{id:guid}/anular")]
    [Authorize(Roles = Roles.TesoreriaEscritura)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Anular(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new AnularCuentaPorPagarCommand(id), cancellationToken);
        return NoContent();
    }

    public sealed record PagarCuentaPorPagarRequest(decimal Monto, Guid BancoId, MedioPago MedioPago);
}
