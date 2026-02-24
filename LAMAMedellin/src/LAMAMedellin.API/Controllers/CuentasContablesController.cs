using LAMAMedellin.Application.Features.Contabilidad.Queries.GetCatalogoCuentas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/cuentas-contables")]
[Authorize]
public sealed class CuentasContablesController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Retorna el Catálogo de Cuentas completo (PUC ESAL) ordenado por código.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CuentaContableDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalogo(CancellationToken cancellationToken)
    {
        var cuentas = await sender.Send(new GetCatalogoCuentasQuery(), cancellationToken);
        return Ok(cuentas);
    }

    /// <summary>
    /// Retorna únicamente las cuentas que admiten asientos contables directos
    /// (PermiteMovimiento = true). Útil para los selectores en formularios de comprobantes.
    /// </summary>
    [HttpGet("asentables")]
    [ProducesResponseType(typeof(List<CuentaContableDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsentables(CancellationToken cancellationToken)
    {
        var cuentas = await sender.Send(new GetCatalogoCuentasQuery(SoloAsentables: true), cancellationToken);
        return Ok(cuentas);
    }
}
