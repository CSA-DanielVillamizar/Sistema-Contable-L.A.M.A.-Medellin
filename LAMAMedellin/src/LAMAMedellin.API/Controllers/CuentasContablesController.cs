using LAMAMedellin.Application.Features.Contabilidad.Queries.GetCuentasContables;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/cuentas-contables")]
[Authorize]
public sealed class CuentasContablesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CuentaContableDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var catalogo = await sender.Send(new GetCuentasContablesQuery(), cancellationToken);
        return Ok(catalogo);
    }
}
