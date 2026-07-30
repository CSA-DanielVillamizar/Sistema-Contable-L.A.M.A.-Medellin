using LAMAMedellin.Application.Features.Tesoreria.Commands.ActualizarCuentaBancaria;
using LAMAMedellin.Application.Features.Tesoreria.Commands.CambiarEstadoCuentaBancaria;
using LAMAMedellin.Application.Features.Tesoreria.Commands.CrearCuentaBancaria;
using LAMAMedellin.Application.Features.Tesoreria.Commands.RegistrarEgreso;
using LAMAMedellin.Application.Features.Tesoreria.Commands.RegistrarIngreso;
using LAMAMedellin.Application.Features.Tesoreria.Queries.GetCuentasBancarias;
using LAMAMedellin.Application.Features.Tesoreria.Queries.GetEgresos;
using MediatR;
using LAMAMedellin.API.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/tesoreria")]
[Authorize(Roles = Roles.TesoreriaLectura)]
public sealed class TesoreriaController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Cuentas bancarias. Por defecto solo las activas, que son las unicas que
    /// admiten movimientos; la pantalla de administracion pide tambien las
    /// inactivas para poder reactivarlas.
    /// </summary>
    [HttpGet("cuentas-bancarias")]
    [ProducesResponseType(typeof(List<CuentaBancariaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCuentasBancarias(
        [FromQuery] bool incluirInactivas,
        CancellationToken cancellationToken)
    {
        var cuentas = await sender.Send(new GetCuentasBancariasQuery(incluirInactivas), cancellationToken);
        return Ok(cuentas);
    }

    [HttpPost("cuentas-bancarias")]
    [Authorize(Roles = Roles.TesoreriaEscritura)]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> CrearCuentaBancaria(
        [FromBody] CrearCuentaBancariaCommand command,
        CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return Created($"/api/tesoreria/cuentas-bancarias/{id}", new { id });
    }

    [HttpPut("cuentas-bancarias/{id:guid}")]
    [Authorize(Roles = Roles.TesoreriaEscritura)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ActualizarCuentaBancaria(
        Guid id,
        [FromBody] ActualizarCuentaBancariaRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new ActualizarCuentaBancariaCommand(id, request.Nombre, request.NumeroCuenta, request.CuentaContableId),
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Baja logica. No hay DELETE: los movimientos ya registrados contra la
    /// cuenta deben seguir existiendo para que el libro cuadre.
    /// </summary>
    [HttpPatch("cuentas-bancarias/{id:guid}/estado")]
    [Authorize(Roles = Roles.TesoreriaEscritura)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CambiarEstadoCuentaBancaria(
        Guid id,
        [FromBody] CambiarEstadoCuentaBancariaRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new CambiarEstadoCuentaBancariaCommand(id, request.EsActivo), cancellationToken);
        return NoContent();
    }

    public sealed record ActualizarCuentaBancariaRequest(
        string Nombre,
        string NumeroCuenta,
        Guid CuentaContableId);

    public sealed record CambiarEstadoCuentaBancariaRequest(bool EsActivo);

    [HttpGet("egresos")]
    [ProducesResponseType(typeof(List<EgresoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEgresos(CancellationToken cancellationToken)
    {
        var egresos = await sender.Send(new GetEgresosQuery(), cancellationToken);
        return Ok(egresos);
    }

    [HttpPost("egresos")]
    [Authorize(Roles = Roles.TesoreriaEscritura)]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> RegistrarEgreso([FromBody] RegistrarEgresoCommand command, CancellationToken cancellationToken)
    {
        var egresoId = await sender.Send(command, cancellationToken);
        return Created($"/api/tesoreria/egresos/{egresoId}", new { id = egresoId });
    }

    [HttpPost("ingresos")]
    [Authorize(Roles = Roles.TesoreriaEscritura)]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> RegistrarIngreso([FromBody] RegistrarIngresoCommand command, CancellationToken cancellationToken)
    {
        var ingresoId = await sender.Send(command, cancellationToken);
        return Created($"/api/tesoreria/ingresos/{ingresoId}", new { id = ingresoId });
    }
}
