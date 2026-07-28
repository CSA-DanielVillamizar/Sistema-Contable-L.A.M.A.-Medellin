using LAMAMedellin.Application.Features.Cartera.Commands.GenerarCarteraMensual;
using LAMAMedellin.Application.Features.Cartera.Commands.CrearConceptoCobro;
using LAMAMedellin.Application.Features.Cartera.Commands.CrearCuentaPorCobrar;
using LAMAMedellin.Application.Features.Cartera.Commands.CrearMiembro;
using LAMAMedellin.Application.Features.Cartera.Commands.RegistrarPagoCartera;
using LAMAMedellin.Application.Features.Cartera.Queries.GetCarteraPendiente;
using LAMAMedellin.Application.Features.Cartera.Queries.GetConceptosCobroLookup;
using LAMAMedellin.Application.Features.Cartera.Queries.GetCuentasPorCobrar;
using LAMAMedellin.Application.Features.Cartera.Queries.GetMiembrosLookup;
using LAMAMedellin.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/cartera")]
[Authorize]
public sealed class CarteraController(ISender sender) : ControllerBase
{
    [HttpPost("miembros")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearMiembro(
        [FromBody] CrearMiembroRequest request,
        CancellationToken cancellationToken)
    {
        var id = await sender.Send(new CrearMiembroCommand(
            request.DocumentoIdentidad,
            request.Nombres,
            request.Apellidos,
            request.Apodo,
            request.FechaIngreso,
            request.TipoSangre,
            request.NombreContactoEmergencia,
            request.TelefonoContactoEmergencia,
            request.MarcaMoto,
            request.ModeloMoto,
            request.Cilindraje,
            request.Placa,
            request.Rango), cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new { id });
    }

    [HttpPost("conceptos-cobro")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearConceptoCobro(
        [FromBody] CrearConceptoCobroRequest request,
        CancellationToken cancellationToken)
    {
        var id = await sender.Send(new CrearConceptoCobroCommand(
            request.Nombre,
            request.ValorCOP,
            request.PeriodicidadMensual,
            request.CuentaContableIngresoId), cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new { id });
    }

    [HttpPost("cuentas-por-cobrar")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearCuentaPorCobrar(
        [FromBody] CrearCuentaPorCobrarRequest request,
        CancellationToken cancellationToken)
    {
        var id = await sender.Send(new CrearCuentaPorCobrarCommand(
            request.MiembroId,
            request.ConceptoCobroId,
            request.FechaEmision,
            request.FechaVencimiento,
            request.ValorTotal), cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new { id });
    }

    [HttpPost("generar-mensual")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerarCarteraMensual(
        [FromBody] GenerarCarteraMensualRequest request,
        CancellationToken cancellationToken)
    {
        var resultado = await sender.Send(new GenerarCarteraMensualCommand(request.Periodo), cancellationToken);

        return Ok(new
        {
            periodo = request.Periodo,
            cuotasGeneradas = resultado,
            mensaje = $"Se han generado {resultado} obligaciones para el periodo {request.Periodo}"
        });
    }

    [HttpGet("pendiente")]
    [ProducesResponseType(typeof(List<CarteraPendienteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCarteraPendiente(CancellationToken cancellationToken)
    {
        var cartera = await sender.Send(new GetCarteraPendienteQuery(), cancellationToken);
        return Ok(cartera);
    }

    [HttpGet("cuentas-por-cobrar")]
    [ProducesResponseType(typeof(List<CuentaPorCobrarDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCuentasPorCobrar(
        [FromQuery] EstadoCuentaPorCobrar? estado,
        [FromQuery] Guid? miembroId,
        CancellationToken cancellationToken)
    {
        var cuentas = await sender.Send(
            new GetCuentasPorCobrarQuery(estado, miembroId),
            cancellationToken);

        return Ok(cuentas);
    }

    [HttpGet("miembros/lookup")]
    [ProducesResponseType(typeof(List<MiembroLookupDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMiembrosLookup(CancellationToken cancellationToken)
    {
        var miembros = await sender.Send(new GetMiembrosLookupQuery(), cancellationToken);
        return Ok(miembros);
    }

    [HttpGet("conceptos-cobro/lookup")]
    [ProducesResponseType(typeof(List<ConceptoCobroLookupDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConceptosCobroLookup(CancellationToken cancellationToken)
    {
        var conceptos = await sender.Send(new GetConceptosCobroLookupQuery(), cancellationToken);
        return Ok(conceptos);
    }

    [HttpPost("cuentas-por-cobrar/{id:guid}/pagos")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegistrarPago(
        Guid id,
        [FromBody] RegistrarPagoRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new RegistrarPagoCarteraCommand(
            id,
            request.Monto,
            request.BancoId), cancellationToken);

        return Ok(new
        {
            mensaje = "Pago registrado correctamente."
        });
    }
}

public sealed record GenerarCarteraMensualRequest(string Periodo);
public sealed record RegistrarPagoRequest(decimal Monto, Guid BancoId);
public sealed record CrearMiembroRequest(
    string DocumentoIdentidad,
    string Nombres,
    string Apellidos,
    string Apodo,
    DateOnly FechaIngreso,
    GrupoSanguineo TipoSangre,
    string NombreContactoEmergencia,
    string TelefonoContactoEmergencia,
    string MarcaMoto,
    string ModeloMoto,
    int Cilindraje,
    string Placa,
    RangoClub Rango);

public sealed record CrearConceptoCobroRequest(
    string Nombre,
    decimal ValorCOP,
    int PeriodicidadMensual,
    Guid CuentaContableIngresoId);

public sealed record CrearCuentaPorCobrarRequest(
    Guid MiembroId,
    Guid ConceptoCobroId,
    DateOnly FechaEmision,
    DateOnly FechaVencimiento,
    decimal ValorTotal);
