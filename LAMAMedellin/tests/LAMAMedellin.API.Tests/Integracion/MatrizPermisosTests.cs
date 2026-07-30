using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LAMAMedellin.API.Tests.Infraestructura;
using Xunit;

namespace LAMAMedellin.API.Tests.Integracion;

/// <summary>
/// Matriz de permisos del BRD (seccion 9), historia 0-4.
///
/// Antes de esto, 12 de los 19 controladores no declaraban rol: cualquier
/// usuario autenticado podia registrar comprobantes, crear miembros o
/// administrar usuarios. Estas pruebas fijan quien puede que, porque una matriz
/// sin pruebas se desarma en el primer refactor sin que nadie se entere.
/// </summary>
public sealed class MatrizPermisosTests(FabricaApiPruebas fabrica) : IClassFixture<FabricaApiPruebas>
{
    [Theory]
    [InlineData("Contador")]
    [InlineData("Tesorero")]
    [InlineData("Operador")]
    [InlineData("Junta")]
    public async Task Solo_Admin_administra_usuarios(string rol)
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente(rol);

        var respuesta = await cliente.GetAsync("/api/usuarios");

        respuesta.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Admin_si_administra_usuarios()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Admin");

        var respuesta = await cliente.GetAsync("/api/usuarios");

        respuesta.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("Junta")]
    [InlineData("Logistica")]
    public async Task Un_rol_sin_permiso_no_registra_comprobantes(string rol)
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente(rol);

        var respuesta = await cliente.PostAsJsonAsync("/api/comprobantes", new { });

        // Lo que importa es que no llegue al handler: 403 antes que 400.
        respuesta.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task La_Junta_puede_consultar_contabilidad_pero_no_asentar()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Junta");

        var lectura = await cliente.GetAsync("/api/cuentas-contables");
        lectura.StatusCode.Should().Be(HttpStatusCode.OK);

        var escritura = await cliente.PostAsJsonAsync("/api/comprobantes", new { });
        escritura.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task La_Junta_no_accede_a_los_datos_de_beneficiarios()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Junta");

        var respuesta = await cliente.GetAsync("/api/beneficiarios");

        // Criterio explicito de la historia 0-4: la Junta recibe agregados, no
        // datos personales de beneficiarios.
        respuesta.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task El_Contador_no_mueve_dinero_pero_si_lo_consulta()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Contador");

        var lectura = await cliente.GetAsync("/api/tesoreria/cuentas-bancarias");
        lectura.StatusCode.Should().Be(HttpStatusCode.OK);

        var escritura = await cliente.PostAsJsonAsync("/api/tesoreria/ingresos", new { });
        escritura.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task El_Tesorero_valida_el_cierre_pero_no_lo_ejecuta()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Tesorero");

        var validar = await cliente.PostAsync("/api/periodos-contables/2026/1/validar", null);
        validar.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);

        var cerrar = await cliente.PostAsync("/api/periodos-contables/2026/1/cerrar", null);
        cerrar.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task El_Contador_ejecuta_el_cierre()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Contador");

        var cerrar = await cliente.PostAsync("/api/periodos-contables/2026/1/cerrar", null);

        cerrar.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task El_estado_del_servicio_es_publico()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CreateClient();

        var respuesta = await cliente.GetAsync("/");

        // Es el health check: si exigiera sesion, el balanceador lo daria por
        // caido.
        respuesta.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Ningun_controlador_queda_sin_declarar_rol()
    {
        var sinRol = typeof(Program).Assembly
            .GetTypes()
            .Where(t => t.Name.EndsWith("Controller", StringComparison.Ordinal) && !t.IsAbstract)
            .Where(t => t.Name != "EstadoController")
            .Where(t => t.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true)
                .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
                .All(a => string.IsNullOrWhiteSpace(a.Roles)))
            .Select(t => t.Name)
            .ToList();

        // EstadoController queda fuera a proposito: es el health check anonimo.
        sinRol.Should().BeEmpty(
            "cada controlador debe declarar su fila de la matriz; sin rol, basta con estar autenticado");
    }
}
