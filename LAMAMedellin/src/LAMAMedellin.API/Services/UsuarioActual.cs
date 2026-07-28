using System.Security.Claims;
using LAMAMedellin.Application.Common.Interfaces.Services;

namespace LAMAMedellin.API.Services;

/// <summary>
/// Resuelve el usuario de la peticion HTTP en curso a partir de los claims del
/// token de Entra ID.
/// </summary>
public sealed class UsuarioActual(IHttpContextAccessor httpContextAccessor) : IUsuarioActual
{
    // Microsoft.Identity.Web mapea algunos claims a las URIs largas de
    // System.Security.Claims, asi que se buscan ambas formas.
    private static readonly string[] ClaimsCandidatos =
    [
        "preferred_username",
        ClaimTypes.Upn,
        ClaimTypes.Email,
        "email",
        "oid",
        "http://schemas.microsoft.com/identity/claims/objectidentifier",
        ClaimTypes.NameIdentifier,
        "sub",
    ];

    public string? Identificador
    {
        get
        {
            var usuario = httpContextAccessor.HttpContext?.User;

            if (usuario?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            foreach (var tipo in ClaimsCandidatos)
            {
                var valor = usuario.FindFirst(tipo)?.Value;

                if (!string.IsNullOrWhiteSpace(valor))
                {
                    return valor.Length > 256 ? valor[..256] : valor;
                }
            }

            return null;
        }
    }
}
