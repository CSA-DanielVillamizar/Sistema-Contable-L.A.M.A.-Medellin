using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace LAMAMedellin.API.Authorization;

/// <summary>
/// Admin satisface cualquier exigencia de rol.
///
/// Hasta ahora Admin figuraba en las veinte agrupaciones de la matriz, una por
/// una. Funcionaba, pero era una convencion que habia que recordar: bastaba con
/// que alguien anadiera un controlador nuevo y no incluyera Admin en su fila
/// para dejar al administrador fuera de su propio sistema, y el fallo solo
/// aparecia cuando alguien intentaba usar esa pantalla.
///
/// Con esto la regla deja de repetirse y pasa a ser parte de la tuberia: toda
/// exigencia de rol, presente o futura, la cumple Admin.
///
/// Solo afecta a los roles. Un endpoint sigue exigiendo sesion iniciada, y la
/// politica que bloquea a los usuarios dados de baja se sigue evaluando aparte,
/// de modo que un Admin desactivado tampoco entra.
/// </summary>
public sealed class AdminSiempreAutorizadoHandler : AuthorizationHandler<RolesAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RolesAuthorizationRequirement requirement)
    {
        if (context.User.IsInRole(Roles.Admin))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
