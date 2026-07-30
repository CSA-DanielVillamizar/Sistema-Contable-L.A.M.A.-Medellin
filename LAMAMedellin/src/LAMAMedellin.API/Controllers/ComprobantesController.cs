using LAMAMedellin.Application.Features.Contabilidad.Commands.RegistrarComprobante;
using MediatR;
using LAMAMedellin.API.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/comprobantes")][Authorize(Roles = Roles.ContabilidadLectura)]
public sealed class ComprobantesController(ISender sender) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Roles.ContabilidadEscritura)]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarComprobanteCommand command, CancellationToken cancellationToken)
    {
        var comprobanteId = await sender.Send(command, cancellationToken);
        return Created($"/api/comprobantes/{comprobanteId}", new { id = comprobanteId });
    }
}
