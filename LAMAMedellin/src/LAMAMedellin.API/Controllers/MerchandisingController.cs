using LAMAMedellin.Application.Features.Merchandising.Commands.CrearProducto;
using LAMAMedellin.Application.Features.Merchandising.Commands.RegistrarEntradaInventario;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

/// <summary>
/// Controller para gestión de merchandising e inventario de la tienda del capítulo.
/// </summary>
[ApiController]
[Route("api/merchandising")]
[Authorize(Roles = "Admin,Tesorero,Inventario")]
public sealed class MerchandisingController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Crea un nuevo producto en el catálogo.
    /// </summary>
    /// <param name="command">Datos del producto a crear</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Id del producto creado</returns>
    [HttpPost("productos")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> CrearProducto(
        [FromBody] CrearProductoCommand command,
        CancellationToken cancellationToken)
    {
        var productoId = await sender.Send(command, cancellationToken);
        return Created($"/api/merchandising/productos/{productoId}", new { id = productoId });
    }

    /// <summary>
    /// Registra una entrada de inventario (compra/fondeo de mercancía).
    /// Realiza de forma atómica: ajuste de stock + registro de movimiento.
    /// </summary>
    /// <param name="productoId">Id del producto</param>
    /// <param name="command">Datos de la entrada de inventario</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Id del movimiento registrado</returns>
    [HttpPost("productos/{productoId}/entradas")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> RegistrarEntrada(
        Guid productoId,
        [FromBody] RegistrarEntradaInventarioCommand command,
        CancellationToken cancellationToken)
    {
        // Asegurar que el productoId del URL coincide con el del comando
        var commandConProductoId = command with { ProductoId = productoId };
        var movimientoId = await sender.Send(commandConProductoId, cancellationToken);
        return Created($"/api/merchandising/productos/{productoId}/movimientos/{movimientoId}", new { id = movimientoId });
    }
}
