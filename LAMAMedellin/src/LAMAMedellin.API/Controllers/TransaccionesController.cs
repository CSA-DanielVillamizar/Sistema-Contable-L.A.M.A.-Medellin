using LAMAMedellin.Application.Features.Transacciones.Commands.RegistrarIngreso;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/transacciones")]
[Authorize]
public sealed class TransaccionesController(ISender sender) : ControllerBase
{
    [HttpPost("ingreso")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> RegistrarIngreso([FromBody] RegistrarIngresoCommand command, CancellationToken cancellationToken)
    {
        var transaccionId = await sender.Send(command, cancellationToken);

        return Created($"/api/transacciones/{transaccionId}", new { id = transaccionId });
    }
}
