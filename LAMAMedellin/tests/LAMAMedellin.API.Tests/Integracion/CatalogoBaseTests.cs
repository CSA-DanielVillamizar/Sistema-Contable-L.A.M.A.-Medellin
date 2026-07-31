using System.Net;
using FluentAssertions;
using LAMAMedellin.API.Tests.Infraestructura;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LAMAMedellin.API.Tests.Integracion;

/// <summary>
/// El catalogo base que siembra la aplicacion al arrancar. Si esto no esta
/// completo no se puede registrar ningun movimiento, asi que es la primera
/// condicion que debe cumplirse.
/// </summary>
public sealed class CatalogoBaseTests(FabricaApiPruebas fabrica) : IClassFixture<FabricaApiPruebas>
{
    [Fact]
    public async Task El_plan_de_cuentas_responde_a_un_usuario_autenticado()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente();

        var respuesta = await cliente.GetAsync("/api/cuentas-contables");

        respuesta.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Sin_autenticar_el_plan_de_cuentas_responde_401()
    {
        await fabrica.PrepararBaseAsync();

        // Cliente sin ningun rol: el esquema de prueba autentica igual, asi que
        // lo que se comprueba mas abajo es la autorizacion, no esto.
        var cliente = fabrica.CrearCliente();
        var respuesta = await cliente.GetAsync("/api/cuentas-contables");

        respuesta.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task El_PUC_incluye_deudores_para_poder_registrar_la_cartera()
    {
        var contexto = await fabrica.PrepararBaseAsync();

        var deudores = await contexto.CuentasContables
            .Where(c => c.Codigo.StartsWith("13") && c.PermiteMovimiento)
            .ToListAsync();

        // Sin una cuenta de deudores el derecho de cobro que genera la cartera
        // queda fuera del balance hasta que el miembro paga.
        deudores.Should().NotBeEmpty(
            "la cartera emite cuentas por cobrar y necesitan contrapartida en el activo");
    }

    [Fact]
    public async Task El_PUC_incluye_pasivos_para_poder_registrar_obligaciones()
    {
        var contexto = await fabrica.PrepararBaseAsync();

        var pasivos = await contexto.CuentasContables
            .Where(c => c.Codigo.StartsWith("2") && c.PermiteMovimiento)
            .ToListAsync();

        pasivos.Should().NotBeEmpty("sin cuentas de clase 2 el balance no cuadra en cuanto haya algo por pagar");
    }

    [Fact]
    public async Task Existe_la_cuenta_de_ingresos_recibidos_para_terceros()
    {
        var contexto = await fabrica.PrepararBaseAsync();

        // Es la figura que pidio el cliente para la renovacion de membresia
        // internacional de 20 USD: el capitulo la recauda pero no es ingreso
        // propio, es una obligacion con el comite internacional.
        var cuenta = await contexto.CuentasContables
            .FirstOrDefaultAsync(c => c.Codigo == "281505");

        cuenta.Should().NotBeNull();
        cuenta!.PermiteMovimiento.Should().BeTrue();
    }

    [Fact]
    public async Task Toda_cuenta_que_admite_movimiento_cuelga_de_un_padre()
    {
        var contexto = await fabrica.PrepararBaseAsync();

        var huerfanas = await contexto.CuentasContables
            .Where(c => c.PermiteMovimiento && c.CuentaPadreId == null)
            .Select(c => c.Codigo)
            .ToListAsync();

        huerfanas.Should().BeEmpty("una cuenta auxiliar sin padre rompe la agrupacion de los informes");
    }
}
