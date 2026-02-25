using LAMAMedellin.Application.Features.Contabilidad.Commands.RegistrarComprobante;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/comprobantes")]
[Authorize]
public sealed class ComprobantesController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarComprobanteCommand command, CancellationToken cancellationToken)
    {
        var comprobanteId = await sender.Send(command, cancellationToken);
        return Created($"/api/comprobantes/{comprobanteId}", new { id = comprobanteId });
    }
}
