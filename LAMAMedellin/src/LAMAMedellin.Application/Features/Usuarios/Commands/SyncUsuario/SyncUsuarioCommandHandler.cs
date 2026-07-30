using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Common.Interfaces.Services;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Usuarios.Commands.SyncUsuario;

public sealed class SyncUsuarioCommandHandler(
    IUsuarioRepository usuarioRepository,
    IConfiguracionSeguridad configuracionSeguridad)
    : IRequestHandler<SyncUsuarioCommand, SyncUsuarioResponseDto>
{
    public async Task<SyncUsuarioResponseDto> Handle(SyncUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await usuarioRepository.GetByEntraObjectIdAsync(request.EntraObjectId, cancellationToken);

        if (usuario is not null)
        {
            return new SyncUsuarioResponseDto(usuario.Id, usuario.Rol.ToString());
        }

        // Quien administra se declara en la configuracion del entorno, no en el
        // codigo: el cargo cambia de persona con el tiempo y tener nombres
        // propios compilados obligaria a un despliegue cada vez.
        var esAdministradorDeclarado = configuracionSeguridad.EsAdministradorInicial(request.Email);

        // Red de seguridad para cuando nadie configuro la lista: con la tabla
        // vacia no existe ningun Admin que pueda repartir roles, asi que el
        // primero en entrar lo recibe. Es una puerta que se cierra sola, y sin
        // ella aplicar la matriz de permisos dejaba la aplicacion inaccesible
        // para todos.
        var esPrimerUsuario = (await usuarioRepository.GetAllAsync(cancellationToken)).Count == 0;

        usuario = new Usuario(
            request.Email,
            request.EntraObjectId,
            esAdministradorDeclarado || esPrimerUsuario ? RolSistema.Admin : RolSistema.Logistica,
            true,
            null);

        await usuarioRepository.AddAsync(usuario, cancellationToken);
        await usuarioRepository.SaveChangesAsync(cancellationToken);

        return new SyncUsuarioResponseDto(usuario.Id, usuario.Rol.ToString());
    }
}
