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
[Route("api/usuarios")]
[Authorize]
public sealed class UsuariosController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Crea el perfil interno del usuario que acaba de autenticarse.
    ///
    /// Basta con estar autenticado, sin rol: es el unico camino para tener uno.
    /// Exigir Admin aqui creaba un bloqueo mutuo del que no se sale, porque
    /// hace falta perfil para tener rol y rol para crear el perfil. Con la
    /// tabla de usuarios vacia, eso dejaba la aplicacion inaccesible para
    /// todos.
    ///
    /// Crear el perfil no concede permisos por si solo: el rol que se asigna es
    /// el mas bajo, y subirlo sigue siendo cosa de un Admin.
    /// </summary>
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
    [Authorize(Roles = Roles.SoloAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AsignarRol(Guid id, [FromBody] AsignarRolRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new AsignarRolCommand(id, request.NuevoRol), cancellationToken);
        return NoContent();
    }

    [HttpGet]
    [Authorize(Roles = Roles.SoloAdmin)]
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
