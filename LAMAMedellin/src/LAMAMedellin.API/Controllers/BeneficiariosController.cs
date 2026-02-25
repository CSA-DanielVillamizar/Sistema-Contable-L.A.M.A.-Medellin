using LAMAMedellin.Application.Features.Beneficiarios.Commands.CreateBeneficiario;
using LAMAMedellin.Application.Features.Beneficiarios.Commands.DeleteBeneficiario;
using LAMAMedellin.Application.Features.Beneficiarios.Commands.UpdateBeneficiario;
using LAMAMedellin.Application.Features.Beneficiarios.Queries.GetBeneficiarioById;
using LAMAMedellin.Application.Features.Beneficiarios.Queries.GetBeneficiarios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/beneficiarios")]
[Authorize]
public sealed class BeneficiariosController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LAMAMedellin.Application.Features.Beneficiarios.Queries.GetBeneficiarios.BeneficiarioDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var beneficiarios = await sender.Send(new GetBeneficiariosQuery(), cancellationToken);
        return Ok(beneficiarios);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LAMAMedellin.Application.Features.Beneficiarios.Queries.GetBeneficiarioById.BeneficiarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var beneficiario = await sender.Send(new GetBeneficiarioByIdQuery(id), cancellationToken);
        if (beneficiario is null)
        {
            return NotFound();
        }

        return Ok(beneficiario);
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] UpsertBeneficiarioRequest request, CancellationToken cancellationToken)
    {
        var id = await sender.Send(
            new CreateBeneficiarioCommand(
                request.NombreCompleto,
                request.TipoDocumento,
                request.NumeroDocumento,
                request.Email,
                request.Telefono,
                request.TieneConsentimientoHabeasData),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpsertBeneficiarioRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateBeneficiarioCommand(
                id,
                request.NombreCompleto,
                request.TipoDocumento,
                request.NumeroDocumento,
                request.Email,
                request.Telefono,
                request.TieneConsentimientoHabeasData),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteBeneficiarioCommand(id), cancellationToken);
        return NoContent();
    }

    public sealed record UpsertBeneficiarioRequest(
        string NombreCompleto,
        string TipoDocumento,
        string NumeroDocumento,
        string Email,
        string Telefono,
        bool TieneConsentimientoHabeasData);
}
