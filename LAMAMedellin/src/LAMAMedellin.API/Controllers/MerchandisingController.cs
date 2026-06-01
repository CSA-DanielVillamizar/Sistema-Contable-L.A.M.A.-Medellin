using LAMAMedellin.Application.Features.Merchandising.Commands.ActualizarImagenProducto;
using LAMAMedellin.Application.Features.Merchandising.Commands.CrearProducto;
using LAMAMedellin.Application.Features.Merchandising.Commands.RegistrarEntradaInventario;
using LAMAMedellin.Application.Features.Merchandising.Commands.RegistrarVentaProducto;
using LAMAMedellin.Application.Features.Merchandising.Queries.GetMovimientosProducto;
using LAMAMedellin.Application.Features.Merchandising.Queries.GetProductos;
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
    [HttpGet("productos")]
    [ProducesResponseType(typeof(List<ProductoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductos(CancellationToken cancellationToken)
    {
        var productos = await sender.Send(new GetProductosQuery(), cancellationToken);
        return Ok(productos);
    }

    [HttpGet("productos/{productoId}/movimientos")]
    [ProducesResponseType(typeof(List<MovimientoInventarioDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMovimientosProducto(Guid productoId, CancellationToken cancellationToken)
    {
        var movimientos = await sender.Send(new GetMovimientosProductoQuery(productoId), cancellationToken);
        return Ok(movimientos);
    }

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

    [HttpPost("productos/{productoId}/ventas")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> RegistrarVenta(
        Guid productoId,
        [FromBody] RegistrarVentaProductoCommand command,
        CancellationToken cancellationToken)
    {
        var commandConProductoId = command with { ProductoId = productoId };
        var comprobanteId = await sender.Send(commandConProductoId, cancellationToken);
        return Created($"/api/merchandising/productos/{productoId}/ventas/{comprobanteId}", new { id = comprobanteId });
    }

    /// <summary>
    /// Sube o reemplaza la imagen de un producto.
    /// Acepta multipart/form-data con un campo "imagen" (jpg, jpeg, png o webp, máx 5 MB).
    /// La imagen se almacena en Azure Blob Storage y la URL pública se persiste en la entidad.
    /// </summary>
    /// <param name="productoId">Id del producto</param>
    /// <param name="imagen">Archivo de imagen</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>URL pública del blob</returns>
    [HttpPost("productos/{productoId}/imagen")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActualizarImagen(
        Guid productoId,
        IFormFile imagen,
        CancellationToken cancellationToken)
    {
        if (imagen is null || imagen.Length == 0)
        {
            return BadRequest(new { title = "Archivo requerido.", detail = "El campo 'imagen' es obligatorio y no puede estar vacío." });
        }

        const long MaxBytes = 5 * 1024 * 1024; // 5 MB
        if (imagen.Length > MaxBytes)
        {
            return BadRequest(new { title = "Archivo demasiado grande.", detail = "El tamaño máximo permitido es 5 MB." });
        }

        await using var stream = imagen.OpenReadStream();

        var command = new ActualizarImagenProductoCommand(
            ProductoId: productoId,
            Imagen: stream,
            NombreArchivo: imagen.FileName,
            ContentType: imagen.ContentType);

        var imageUrl = await sender.Send(command, cancellationToken);

        return Ok(new { imageUrl });
    }
}
