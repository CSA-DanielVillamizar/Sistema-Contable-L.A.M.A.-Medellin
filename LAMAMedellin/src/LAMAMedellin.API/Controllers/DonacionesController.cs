using LAMAMedellin.Application.Features.Donaciones.Commands.CrearDonante;
using LAMAMedellin.Application.Features.Donaciones.Commands.RegistrarDonacion;
using LAMAMedellin.Application.Common.Interfaces.Services;
using LAMAMedellin.Application.Features.Donaciones.Queries.GetCertificadoDonacion;
using LAMAMedellin.Application.Features.Donaciones.Commands.ActualizarCampana;
using LAMAMedellin.Application.Features.Donaciones.Commands.CambiarEstadoCampana;
using LAMAMedellin.Application.Features.Donaciones.Commands.CrearCampana;
using LAMAMedellin.Application.Features.Donaciones.Queries.GetCampanas;
using LAMAMedellin.Application.Features.Donaciones.Queries.GetDonaciones;
using LAMAMedellin.Application.Features.Donaciones.Queries.GetDonantes;
using MediatR;
using LAMAMedellin.API.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/donaciones")][Authorize(Roles = Roles.TesoreriaLectura)]
public sealed class DonacionesController(ISender sender, ICertificadoDonacionService certificadoDonacionService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<DonacionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDonaciones(
        [FromQuery] DateOnly? desde,
        [FromQuery] DateOnly? hasta,
        [FromQuery] Guid? donanteId,
        [FromQuery] Guid? centroCostoId,
        [FromQuery] bool? certificadoEmitido,
        CancellationToken cancellationToken)
    {
        var donaciones = await sender.Send(new GetDonacionesQuery(desde, hasta, donanteId, centroCostoId, certificadoEmitido), cancellationToken);
        return Ok(donaciones);
    }

    /// <summary>Campanas de donacion con su avance frente a la meta (historia 2-1).</summary>
    [HttpGet("campanas")]
    [ProducesResponseType(typeof(IReadOnlyList<CampanaDonacionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCampanas(
        [FromQuery] bool incluirCerradas,
        CancellationToken cancellationToken)
    {
        var campanas = await sender.Send(new GetCampanasQuery(incluirCerradas), cancellationToken);
        return Ok(campanas);
    }

    [HttpPost("campanas")]
    [Authorize(Roles = Roles.TesoreriaEscritura)]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> CrearCampana(
        [FromBody] CrearCampanaCommand command,
        CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return Created($"/api/donaciones/campanas/{id}", new { id });
    }

    [HttpPut("campanas/{id:guid}")]
    [Authorize(Roles = Roles.TesoreriaEscritura)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ActualizarCampana(
        Guid id,
        [FromBody] ActualizarCampanaRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new ActualizarCampanaCommand(id, request.Nombre, request.Descripcion, request.MetaCOP,
                request.FechaInicio, request.FechaFin),
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Cierra o reabre. No hay borrado: lo recaudado bajo la campana debe
    /// seguir existiendo.
    /// </summary>
    [HttpPatch("campanas/{id:guid}/estado")]
    [Authorize(Roles = Roles.TesoreriaEscritura)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CambiarEstadoCampana(
        Guid id,
        [FromBody] CambiarEstadoCampanaRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new CambiarEstadoCampanaCommand(id, request.Activa), cancellationToken);
        return NoContent();
    }

    public sealed record ActualizarCampanaRequest(
        string Nombre,
        string Descripcion,
        decimal MetaCOP,
        DateOnly FechaInicio,
        DateOnly FechaFin);

    public sealed record CambiarEstadoCampanaRequest(bool Activa);

    [HttpGet("donantes")]
    [ProducesResponseType(typeof(List<DonanteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDonantes(CancellationToken cancellationToken)
    {
        var donantes = await sender.Send(new GetDonantesQuery(), cancellationToken);
        return Ok(donantes);
    }

    [HttpPost("donantes")]
    [Authorize(Roles = Roles.TesoreriaEscritura)]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> CrearDonante([FromBody] CrearDonanteCommand command, CancellationToken cancellationToken)
    {
        var donanteId = await sender.Send(command, cancellationToken);
        return Created($"/api/donaciones/donantes/{donanteId}", new { id = donanteId });
    }

    [HttpPost]
    [Authorize(Roles = Roles.TesoreriaEscritura)]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> RegistrarDonacion([FromBody] RegistrarDonacionCommand command, CancellationToken cancellationToken)
    {
        var donacionId = await sender.Send(command, cancellationToken);
        return Created($"/api/donaciones/{donacionId}", new { id = donacionId });
    }

    [HttpGet("{id:guid}/certificado")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCertificado(Guid id, CancellationToken cancellationToken)
    {
        var certificado = await sender.Send(new GetCertificadoDonacionQuery(id), cancellationToken);
        if (certificado is null)
        {
            return NotFound();
        }

        var fundacion = new
        {
            Nombre = "Fundación L.A.M.A. Medellín",
            Nit = "902.007.705-8",
            Direccion = "Calle 8 Sur No. 43 B 112",
            Ciudad = "Medellín, Antioquia, Colombia"
        };

        return Ok(new
        {
            Fundacion = fundacion,
            Donante = new
            {
                certificado.DonanteId,
                certificado.NombreDonante,
                certificado.TipoDocumento,
                certificado.NumeroDocumento,
                certificado.Email
            },
            Monto = new
            {
                ValorCOP = certificado.MontoCOP,
                EnLetras = certificado.MontoEnLetras
            },
            certificado.FormaDonacion,
            certificado.MedioPagoODescripcion,
            certificado.AnioGravable,
            certificado.Fecha,
            certificado.CodigoVerificacion
        });
    }

    [HttpGet("{id:guid}/certificado/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DescargarCertificadoPdf(Guid id, CancellationToken cancellationToken)
    {
        var certificado = await sender.Send(new GetCertificadoDonacionQuery(id), cancellationToken);
        if (certificado is null)
        {
            return NotFound();
        }

        var pdf = certificadoDonacionService.GenerarPdf(certificado);
        return File(pdf, "application/pdf", $"certificado-donacion-{certificado.CodigoVerificacion}.pdf");
    }
}
