using LAMAMedellin.Application.Features.Merchandising.Commands.ProcesarVenta;
using LAMAMedellin.Application.Features.Merchandising.Queries.GetVentas;
using LAMAMedellin.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/ventas")]
[Authorize]
public sealed class VentasController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<VentaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var ventas = await sender.Send(new GetVentasQuery(), cancellationToken);
        return Ok(ventas);
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] ProcesarVentaRequest request, CancellationToken cancellationToken)
    {
        var numeroFacturaInterna = $"VTA-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

        var id = await sender.Send(
            new ProcesarVentaCommand(
                numeroFacturaInterna,
                request.CompradorId,
                request.MetodoPago,
                request.Detalles
                    .Select(detalle => new ProcesarVentaDetalleDto(detalle.ArticuloId, detalle.Cantidad))
                    .ToList()),
            cancellationToken);

        return CreatedAtAction(nameof(Post), new { id }, new { id });
    }

    public sealed record ProcesarVentaRequest(
        Guid? CompradorId,
        MetodoPagoVenta MetodoPago,
        IReadOnlyList<ProcesarVentaDetalleRequest> Detalles);

    public sealed record ProcesarVentaDetalleRequest(
        Guid ArticuloId,
        int Cantidad);
}
