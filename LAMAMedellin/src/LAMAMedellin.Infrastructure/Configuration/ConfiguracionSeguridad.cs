using LAMAMedellin.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace LAMAMedellin.Infrastructure.Configuration;

/// <summary>
/// Lee los administradores iniciales de la configuracion del entorno.
///
/// En local se declaran en appsettings.Development.json, que no se versiona.
/// En Azure, como ajuste de aplicacion con la forma
/// Seguridad__AdministradoresIniciales__0. En ninguno de los dos casos el
/// correo queda escrito en el codigo.
/// </summary>
public sealed class ConfiguracionSeguridad(IConfiguration configuration) : IConfiguracionSeguridad
{
    public const string ClaveAdministradores = "Seguridad:AdministradoresIniciales";

    public bool EsAdministradorInicial(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        // GetChildren y no Get<string[]>: el binder vive en un paquete aparte
        // que este proyecto no referencia, y no vale la pena traerlo por esto.
        var declarados = configuration.GetSection(ClaveAdministradores)
            .GetChildren()
            .Select(x => x.Value)
            .Where(x => !string.IsNullOrWhiteSpace(x));

        // Comparacion sin distinguir mayusculas: el correo que llega del token
        // no siempre respeta como lo escribio quien configuro la lista.
        return declarados.Any(d => string.Equals(d?.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}
