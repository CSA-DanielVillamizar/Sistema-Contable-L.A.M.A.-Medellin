using LAMAMedellin.Application.Features.Merchandising.Commands.CreateArticulo;
using LAMAMedellin.Application.Features.Merchandising.Queries.GetArticulos;
using LAMAMedellin.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/articulos")]
[Authorize]
public sealed class ArticulosController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ArticuloDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var articulos = await sender.Send(new GetArticulosQuery(), cancellationToken);
        return Ok(articulos);
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] CreateArticuloRequest request, CancellationToken cancellationToken)
    {
        var id = await sender.Send(
            new CreateArticuloCommand(
                request.Nombre,
                request.SKU,
                request.Descripcion,
                request.Categoria,
                request.PrecioVenta,
                request.CostoPromedio,
                request.StockActual,
                request.CuentaContableIngresoId),
            cancellationToken);

        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    public sealed record CreateArticuloRequest(
        string Nombre,
        string SKU,
        string Descripcion,
        CategoriaArticulo Categoria,
        decimal PrecioVenta,
        decimal CostoPromedio,
        int StockActual,
        Guid CuentaContableIngresoId);
}
