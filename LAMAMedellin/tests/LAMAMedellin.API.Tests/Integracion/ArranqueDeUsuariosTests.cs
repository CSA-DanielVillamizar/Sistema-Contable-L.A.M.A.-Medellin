using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LAMAMedellin.API.Tests.Infraestructura;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LAMAMedellin.API.Tests.Integracion;

/// <summary>
/// Arranque del sistema: como consigue su rol el primer usuario.
///
/// Estas pruebas existen por una regresion concreta. Al aplicar la matriz de
/// permisos, todos los controladores pasaron a exigir rol, incluido el
/// endpoint que crea el perfil interno. Con la tabla de usuarios vacia eso
/// producia un bloqueo mutuo del que no se sale: hacia falta perfil para tener
/// rol y rol para crear el perfil. La aplicacion quedaba inaccesible para
/// todos y devolvia 403 en cada pantalla.
/// </summary>
public sealed class ArranqueDeUsuariosTests(FabricaApiPruebas fabrica) : IClassFixture<FabricaApiPruebas>
{
    private const string Ruta = "/api/usuarios/sync";

    private static object Perfil(string sufijo) => new
    {
        email = $"usuario{sufijo}@lamamedellin.org",
        entraObjectId = $"oid-{sufijo}",
        nombres = $"Usuario {sufijo}",
    };

    [Fact]
    public async Task Un_usuario_sin_rol_puede_crear_su_perfil()
    {
        await fabrica.PrepararBaseAsync();

        // Cliente autenticado pero sin ningun rol: es la situacion de cualquiera
        // que entra por primera vez.
        var cliente = fabrica.CreateClient();

        var respuesta = await cliente.PostAsJsonAsync(Ruta, Perfil(Guid.NewGuid().ToString("N")[..6]));

        respuesta.StatusCode.Should().NotBe(HttpStatusCode.Forbidden,
            "exigir rol aqui deja la aplicacion inaccesible para todos");
        respuesta.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// Base propia y no la compartida de la clase: "primer usuario" depende del
    /// estado global, y las demas pruebas de este archivo ya crean usuarios.
    /// </summary>
    [Fact]
    public async Task El_primer_usuario_recibe_Admin_para_poder_repartir_roles()
    {
        using var propia = new FabricaApiPruebas();
        var contexto = await propia.PrepararBaseAsync();
        (await contexto.Usuarios.CountAsync()).Should().Be(0, "la prueba parte de una base sin usuarios");

        var cliente = propia.CreateClient();
        var respuesta = await cliente.PostAsJsonAsync(Ruta, Perfil("primero"));

        var creado = await respuesta.Content.ReadFromJsonAsync<SyncRespuesta>();
        creado!.Rol.Should().Be("Admin");
    }

    [Fact]
    public async Task El_segundo_usuario_ya_no_recibe_Admin()
    {
        using var propia = new FabricaApiPruebas();
        await propia.PrepararBaseAsync();
        var cliente = propia.CreateClient();

        await cliente.PostAsJsonAsync(Ruta, Perfil("inicial"));

        var segunda = await cliente.PostAsJsonAsync(Ruta, Perfil("segundo"));
        var creado = await segunda.Content.ReadFromJsonAsync<SyncRespuesta>();

        // La puerta se cierra sola: en cuanto hay un usuario, el siguiente
        // entra con el rol mas bajo y depende de que alguien lo promueva.
        creado!.Rol.Should().NotBe("Admin");
    }

    [Fact]
    public async Task Sincronizar_dos_veces_no_duplica_ni_cambia_el_rol()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CreateClient();

        var primera = await cliente.PostAsJsonAsync(Ruta, Perfil("repetido"));
        var segunda = await cliente.PostAsJsonAsync(Ruta, Perfil("repetido"));

        var uno = await primera.Content.ReadFromJsonAsync<SyncRespuesta>();
        var dos = await segunda.Content.ReadFromJsonAsync<SyncRespuesta>();

        dos!.Id.Should().Be(uno!.Id);
        dos.Rol.Should().Be(uno.Rol);
    }

    [Fact]
    public async Task Administrar_usuarios_sigue_exigiendo_Admin()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Contador");

        var respuesta = await cliente.GetAsync("/api/usuarios");

        // Abrir el arranque no debe abrir la administracion de usuarios.
        respuesta.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Un_correo_declarado_en_configuracion_recibe_Admin_aunque_no_sea_el_primero()
    {
        using var propia = new FabricaApiPruebas();
        await propia.PrepararBaseAsync();
        var cliente = propia.CreateClient();

        // Alguien entra antes y agota la regla del primer usuario.
        await cliente.PostAsJsonAsync(Ruta, Perfil("ocupa"));

        var respuesta = await cliente.PostAsJsonAsync(Ruta, new
        {
            email = FabricaApiPruebas.CorreoAdministradorDeclarado,
            entraObjectId = "oid-declarado",
            nombres = "Administrador declarado",
        });

        var creado = await respuesta.Content.ReadFromJsonAsync<SyncRespuesta>();

        // Quien administra se declara en configuracion, no depende del orden de
        // llegada ni queda escrito en el codigo.
        creado!.Rol.Should().Be("Admin");
    }

    private sealed record SyncRespuesta(Guid Id, string Rol);
}
