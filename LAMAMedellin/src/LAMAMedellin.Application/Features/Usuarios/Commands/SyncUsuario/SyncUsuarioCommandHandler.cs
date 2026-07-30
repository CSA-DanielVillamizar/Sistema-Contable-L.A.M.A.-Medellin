using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Usuarios.Commands.SyncUsuario;

public sealed class SyncUsuarioCommandHandler(IUsuarioRepository usuarioRepository)
    : IRequestHandler<SyncUsuarioCommand, SyncUsuarioResponseDto>
{
    public async Task<SyncUsuarioResponseDto> Handle(SyncUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await usuarioRepository.GetByEntraObjectIdAsync(request.EntraObjectId, cancellationToken);

        if (usuario is not null)
        {
            return new SyncUsuarioResponseDto(usuario.Id, usuario.Rol.ToString());
        }

        // Arranque del sistema: con la tabla vacia no existe ningun Admin que
        // pueda repartir roles, de modo que el primero en entrar lo recibe. Es
        // una puerta que se cierra sola: en cuanto hay un usuario, el siguiente
        // entra con el rol mas bajo y depende de que alguien lo promueva.
        //
        // Sin esto, aplicar la matriz de permisos dejaba la aplicacion
        // inaccesible para todos, porque hacia falta rol para cualquier cosa y
        // no habia forma de conseguir el primero.
        var esPrimerUsuario = (await usuarioRepository.GetAllAsync(cancellationToken)).Count == 0;

        usuario = new Usuario(
            request.Email,
            request.EntraObjectId,
            esPrimerUsuario ? RolSistema.Admin : RolSistema.Logistica,
            true,
            null);

        await usuarioRepository.AddAsync(usuario, cancellationToken);
        await usuarioRepository.SaveChangesAsync(cancellationToken);

        return new SyncUsuarioResponseDto(usuario.Id, usuario.Rol.ToString());
    }
}
