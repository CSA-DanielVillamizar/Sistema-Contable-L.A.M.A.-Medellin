using LAMAMedellin.API.Authorization;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Common.Interfaces.Services;
using LAMAMedellin.Application.Features.Recibos.Queries.VerificarRecibo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

/// <summary>
/// Recibos en PDF con QR y su verificacion publica (historia 1-7).
/// </summary>
[ApiController]
[Route("api/recibos")]
[Authorize(Roles = Roles.TesoreriaLectura)]
public sealed class RecibosController(
    ISender sender,
    IComprobanteRepository comprobanteRepository,
    IReciboService reciboService,
    IConfiguration configuration) : ControllerBase
{
    [HttpGet("{numeroConsecutivo}/pdf")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPdf(string numeroConsecutivo, CancellationToken cancellationToken)
    {
        var comprobante = await comprobanteRepository.GetPorConsecutivoAsync(numeroConsecutivo, cancellationToken);

        if (comprobante is null)
        {
            return NotFound();
        }

        var valor = comprobante.AsientosContables.Sum(a => a.Debe);

        var pdf = reciboService.GenerarPdf(new DatosRecibo(
            comprobante.NumeroConsecutivo,
            comprobante.Fecha,
            comprobante.AsientosContables.FirstOrDefault()?.Referencia ?? "No especificado",
            comprobante.Descripcion,
            valor,
            comprobante.AsientosContables.FirstOrDefault()?.CentroCosto?.Nombre ?? "No especificado",
            comprobante.TipoComprobante.ToString(),
            comprobante.NumeroConsecutivo,
            ConstruirUrlVerificacion(comprobante.NumeroConsecutivo)));

        return File(pdf, "application/pdf", $"recibo-{comprobante.NumeroConsecutivo}.pdf");
    }

    /// <summary>
    /// Verificacion publica: es a donde apunta el QR. Va sin autenticacion a
    /// proposito, porque quien recibe un recibo en papel no tiene cuenta en el
    /// sistema. Por eso la respuesta se limita a consecutivo, fecha, valor y
    /// estado, sin tercero ni concepto ni centro de costo.
    /// </summary>
    [HttpGet("verificar/{numeroConsecutivo}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReciboVerificadoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Verificar(string numeroConsecutivo, CancellationToken cancellationToken)
    {
        var recibo = await sender.Send(new VerificarReciboQuery(numeroConsecutivo), cancellationToken);

        return recibo is null ? NotFound(new { mensaje = "No existe un recibo con ese numero." }) : Ok(recibo);
    }

    private string ConstruirUrlVerificacion(string numeroConsecutivo)
    {
        // Apunta a la pagina de verificacion, no al endpoint JSON: quien
        // escanea el QR con el movil debe ver algo legible, no un objeto crudo.
        //
        // La base es configurable porque el recibo se imprime y el enlace tiene
        // que seguir sirviendo desde fuera de la red del capitulo.
        var baseUrl = configuration["Recibos:UrlVerificacionBase"]
            ?? "http://localhost:3000/verificar";

        return $"{baseUrl.TrimEnd('/')}/{numeroConsecutivo}";
    }
}
