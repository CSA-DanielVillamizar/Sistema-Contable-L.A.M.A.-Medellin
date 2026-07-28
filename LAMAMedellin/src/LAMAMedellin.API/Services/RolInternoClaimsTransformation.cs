using System.Security.Claims;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using Microsoft.AspNetCore.Authentication;

namespace LAMAMedellin.API.Services;

/// <summary>
/// Proyecta el rol interno del usuario como claim de rol en cada peticion
/// autenticada.
///
/// Sin esto, [Authorize(Roles = "Tesorero")] evaluaba unicamente los app roles
/// del token de Entra, mientras Usuario.Rol se guardaba en base de datos y se
/// administraba desde la pantalla de seguridad. El resultado era que asignar un
/// rol en la aplicacion no cambiaba nada de lo que el usuario podia hacer: un
/// control de seguridad que aparentaba funcionar.
///
/// Tambien marca si el perfil interno esta deshabilitado, para que la politica
/// de autorizacion lo bloquee aunque Entra lo siga autenticando (historia 0-2).
/// </summary>
public sealed class RolInternoClaimsTransformation(
    IUsuarioRepository usuarioRepository,
    IConfiguration configuration,
    ILogger<RolInternoClaimsTransformation> logger) : IClaimsTransformation
{
    /// <summary>
    /// Cuando esta activo, los app roles que traiga el token de Entra se
    /// descartan y solo vale el rol interno. Es el estado final que pide
    /// EPIC 01 ("los roles se manejan internamente").
    ///
    /// Viene apagado a proposito: encenderlo antes de haber asignado rol
    /// interno a todos los usuarios los deja a todos sin permisos. La
    /// secuencia segura es desplegar, asignar los roles desde la pantalla de
    /// seguridad, verificar, y recien entonces encenderlo.
    /// </summary>
    private const string ClaveRolesExclusivos = "Seguridad:RolesInternosExclusivos";

    /// <summary>Marca de origen para no acumular claims si se transforma dos veces.</summary>
    public const string OrigenRolInterno = "rol-interno";

    /// <summary>
    /// "false" cuando el perfil interno existe y esta deshabilitado.
    /// Si el claim no esta presente, el usuario aun no tiene perfil interno:
    /// eso NO se bloquea, porque el endpoint que crea el perfil tambien exige
    /// autenticacion y se llegaria a un bloqueo mutuo.
    /// </summary>
    public const string ClaimUsuarioActivo = "lama:usuario-interno-activo";

    private static readonly string[] ClaimsObjectId =
    [
        "oid",
        "http://schemas.microsoft.com/identity/claims/objectidentifier",
    ];

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return principal;
        }

        if (principal.HasClaim(claim => claim.Issuer == OrigenRolInterno))
        {
            return principal;
        }

        var objectId = ClaimsObjectId
            .Select(tipo => principal.FindFirst(tipo)?.Value)
            .FirstOrDefault(valor => !string.IsNullOrWhiteSpace(valor));

        if (string.IsNullOrWhiteSpace(objectId))
        {
            return principal;
        }

        var rolesInternosExclusivos = configuration.GetValue<bool>(ClaveRolesExclusivos);

        var usuario = await usuarioRepository.GetByEntraObjectIdAsync(objectId);

        if (usuario is null)
        {
            // Autenticado en Entra pero sin perfil interno todavia. No se le
            // concede ningun rol, asi que solo alcanza endpoints que se
            // conformen con estar autenticado, entre ellos el de sincronizacion
            // que le creara el perfil.
            logger.LogInformation("Usuario autenticado sin perfil interno; no se conceden roles.");
            return principal;
        }

        // Un usuario dado de baja pierde los roles vengan de donde vengan.
        var descartarRolesDelToken = rolesInternosExclusivos || !usuario.EsActivo;

        var claimsBase = descartarRolesDelToken
            ? principal.Claims.Where(claim => claim.Type != ClaimTypes.Role && claim.Type != "roles")
            : principal.Claims;

        var identidad = new ClaimsIdentity(claimsBase, principal.Identity.AuthenticationType);

        identidad.AddClaim(new Claim(
            ClaimUsuarioActivo,
            usuario.EsActivo ? "true" : "false",
            ClaimValueTypes.String,
            OrigenRolInterno));

        if (usuario.EsActivo)
        {
            identidad.AddClaim(new Claim(
                ClaimTypes.Role,
                usuario.Rol.ToString(),
                ClaimValueTypes.String,
                OrigenRolInterno));
        }
        else
        {
            logger.LogWarning("Usuario interno deshabilitado; se le retiran todos los roles.");
        }

        return new ClaimsPrincipal(identidad);
    }
}
