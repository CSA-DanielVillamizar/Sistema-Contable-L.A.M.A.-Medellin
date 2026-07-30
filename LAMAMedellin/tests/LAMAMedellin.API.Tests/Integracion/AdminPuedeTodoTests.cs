using System.Net;
using System.Reflection;
using FluentAssertions;
using LAMAMedellin.API.Tests.Infraestructura;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace LAMAMedellin.API.Tests.Integracion;

/// <summary>
/// Admin no puede quedar fuera de ninguna parte del sistema.
///
/// Hasta ahora eso dependia de acordarse de incluir Admin en cada fila de la
/// matriz. Funcionaba, pero un controlador nuevo que lo olvidara dejaba al
/// administrador sin acceso a su propia aplicacion, y el fallo solo aparecia
/// cuando alguien intentaba usar esa pantalla.
/// </summary>
public sealed class AdminPuedeTodoTests(FabricaApiPruebas fabrica) : IClassFixture<FabricaApiPruebas>
{
    /// <summary>
    /// Recorre por reflexion todas las exigencias de rol declaradas en los
    /// controladores. Comprobar una muestra no sirve: lo que se quiere evitar
    /// es justamente el sitio que nadie reviso.
    /// </summary>
    [Fact]
    public void Ninguna_exigencia_de_rol_deja_fuera_a_Admin()
    {
        var sinAdmin = new List<string>();

        var controladores = typeof(Program).Assembly
            .GetTypes()
            .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var controlador in controladores)
        {
            foreach (var atributo in Atributos(controlador))
            {
                if (!Admite(atributo, "Admin"))
                {
                    sinAdmin.Add($"{controlador.Name} (clase): {atributo.Roles}");
                }
            }

            foreach (var accion in controlador.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                foreach (var atributo in Atributos(accion))
                {
                    if (!Admite(atributo, "Admin"))
                    {
                        sinAdmin.Add($"{controlador.Name}.{accion.Name}: {atributo.Roles}");
                    }
                }
            }
        }

        sinAdmin.Should().BeEmpty(
            "Admin debe poder operar todo el sistema; si aparece algo aqui, falta incluirlo en esa fila de la matriz");
    }

    [Theory]
    [InlineData("/api/cuentas-contables")]
    [InlineData("/api/comprobantes")]
    [InlineData("/api/tesoreria/cuentas-bancarias")]
    [InlineData("/api/cartera/cuentas-por-cobrar")]
    [InlineData("/api/cuentas-por-pagar")]
    [InlineData("/api/donaciones")]
    [InlineData("/api/donaciones/campanas")]
    [InlineData("/api/miembros")]
    [InlineData("/api/proyectos")]
    [InlineData("/api/proyectos/rendicion")]
    [InlineData("/api/beneficiarios")]
    [InlineData("/api/eventos")]
    [InlineData("/api/merchandising/productos")]
    [InlineData("/api/merchandising/reporte")]
    [InlineData("/api/usuarios")]
    [InlineData("/api/periodos-contables")]
    [InlineData("/api/anulaciones")]
    [InlineData("/api/configuracion/mapeo-contable")]
    [InlineData("/api/configuracion/tarifas")]
    [InlineData("/api/transacciones/bancos")]
    public async Task Admin_alcanza_cada_modulo(string ruta)
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Admin");

        var respuesta = await cliente.GetAsync(ruta);

        respuesta.StatusCode.Should().NotBe(HttpStatusCode.Forbidden, $"Admin debe poder consultar {ruta}");
    }

    [Fact]
    public async Task Un_rol_inventado_no_hereda_los_permisos_de_Admin()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("RolQueNoExiste");

        var respuesta = await cliente.GetAsync("/api/usuarios");

        // Abrir la puerta a Admin no debe abrirsela a cualquiera.
        respuesta.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private static IEnumerable<AuthorizeAttribute> Atributos(MemberInfo miembro)
    {
        return miembro
            .GetCustomAttributes(typeof(AuthorizeAttribute), true)
            .Cast<AuthorizeAttribute>()
            .Where(a => !string.IsNullOrWhiteSpace(a.Roles));
    }

    private static bool Admite(AuthorizeAttribute atributo, string rol)
    {
        return atributo.Roles!
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Contains(rol, StringComparer.OrdinalIgnoreCase);
    }
}
