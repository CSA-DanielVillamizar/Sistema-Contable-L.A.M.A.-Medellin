using LAMAMedellin.Application.Features.Usuarios.Commands.AsignarRol;
using LAMAMedellin.Application.Features.Usuarios.Commands.SyncUsuario;
using LAMAMedellin.Application.Features.Usuarios.Queries.GetUsuarios;
using LAMAMedellin.Domain.Enums;
using MediatR;
using LAMAMedellin.API.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.API.Controllers;

[ApiController]
[Route("api/usuarios")][Authorize(Roles = Roles.SoloAdmin)]
public sealed class UsuariosController(ISender sender) : ControllerBase
{
    [HttpPost("sync")]
    [ProducesResponseType(typeof(SyncUsuarioResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Sync([FromBody] SyncUsuarioRequest request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new SyncUsuarioCommand(
                request.Email,
                request.EntraObjectId,
                request.Nombres),
            cancellationToken);

        return Ok(response);
    }

    [HttpPut("{id:guid}/rol")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AsignarRol(Guid id, [FromBody] AsignarRolRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new AsignarRolCommand(id, request.NuevoRol), cancellationToken);
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UsuarioDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var usuarios = await sender.Send(new GetUsuariosQuery(), cancellationToken);
        return Ok(usuarios);
    }

    public sealed record SyncUsuarioRequest(
        string Email,
        string EntraObjectId,
        string Nombres);

    public sealed record AsignarRolRequest(
        RolSistema NuevoRol);
}
