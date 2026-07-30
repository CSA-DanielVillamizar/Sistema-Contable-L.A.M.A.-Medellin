using LAMAMedellin.Application.Features.Miembros.Commands.CreateMiembro;
using LAMAMedellin.Application.Features.Miembros.Commands.DeleteMiembro;
using LAMAMedellin.Application.Features.Miembros.Commands.UpdateMiembro;
using LAMAMedellin.Application.Features.Miembros.Queries.GetMiembroById;
using LAMAMedellin.Application.Features.Miembros.Queries.GetMiembros;
using LAMAMedellin.Domain.Enums;
using MediatR;
using LAMAMedellin.API.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/miembros")][Authorize(Roles = Roles.MiembrosLectura)]
public sealed class MiembrosController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LAMAMedellin.Application.Features.Miembros.Queries.GetMiembros.MiembroDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var miembros = await sender.Send(new GetMiembrosQuery(), cancellationToken);
        return Ok(miembros);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LAMAMedellin.Application.Features.Miembros.Queries.GetMiembroById.MiembroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var miembro = await sender.Send(new GetMiembroByIdQuery(id), cancellationToken);
        if (miembro is null)
        {
            return NotFound();
        }

        return Ok(miembro);
    }

    [HttpPost]
    [Authorize(Roles = Roles.MiembrosEscritura)]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] UpsertMiembroRequest request, CancellationToken cancellationToken)
    {
        var id = await sender.Send(
            new CreateMiembroCommand(
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
                request.Rango),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.MiembrosEscritura)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpsertMiembroRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateMiembroCommand(
                id,
                request.TipoSangre,
                request.NombreContactoEmergencia,
                request.TelefonoContactoEmergencia,
                request.MarcaMoto,
                request.ModeloMoto,
                request.Cilindraje,
                request.Placa,
                request.Rango,
                request.EsActivo),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.MiembrosEscritura)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteMiembroCommand(id), cancellationToken);
        return NoContent();
    }

    public sealed record UpsertMiembroRequest(
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
        RangoClub Rango,
        bool EsActivo = true);
}
