using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LAMAMedellin.API.Tests.Infraestructura;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LAMAMedellin.API.Tests.Integracion;

/// <summary>
/// Reporte de inventario, ventas y utilidad (historia 4-3).
///
/// La utilidad exige conocer el costo, que hasta ahora no se guardaba: solo se
/// sabia cuanto se habia vendido, no cuanto se habia ganado.
/// </summary>
public sealed class ReporteInventarioTests(FabricaApiPruebas fabrica) : IClassFixture<FabricaApiPruebas>
{
    private const string Ruta = "/api/merchandising/reporte";

    private async Task<(Guid ProductoId, Guid BancoId)> CrearProductoAsync(
        HttpClient cliente,
        decimal precioVenta,
        int stockInicial,
        int minima)
    {
        var contexto = await fabrica.PrepararBaseAsync();

        var cuentaIngreso = await contexto.CuentasContables
            .FirstAsync(c => c.Codigo.StartsWith("4") && c.PermiteMovimiento);
        var banco = await contexto.Bancos.FirstAsync(b => b.EsActivo);

        var respuesta = await cliente.PostAsJsonAsync("/api/merchandising/productos", new
        {
            nombre = $"Producto {Guid.NewGuid():N}"[..18],
            codigoSKU = $"SKU{Guid.NewGuid():N}"[..12],
            precioVenta,
            cantidadEnStock = stockInicial,
            cantidadMinima = minima,
            cuentaContableIngresoId = cuentaIngreso.Id,
        });

        respuesta.StatusCode.Should().Be(HttpStatusCode.Created);
        var creado = await respuesta.Content.ReadFromJsonAsync<RespuestaId>();

        return (creado!.Id, banco.Id);
    }

    [Fact]
    public async Task El_reporte_responde_aunque_no_haya_movimientos()
    {
        await fabrica.PrepararBaseAsync();
        var cliente = fabrica.CrearCliente("Admin");

        var respuesta = await cliente.GetAsync(Ruta);

        respuesta.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Una_entrada_con_costo_define_el_costo_promedio_y_el_valor_del_inventario()
    {
        var cliente = fabrica.CrearCliente("Admin");
        var (productoId, _) = await CrearProductoAsync(cliente, precioVenta: 50_000m, stockInicial: 0, minima: 2);

        var entrada = await cliente.PostAsJsonAsync($"/api/merchandising/productos/{productoId}/entradas", new
        {
            cantidad = 10,
            fecha = DateTime.UtcNow,
            observaciones = "Compra inicial",
            costoUnitario = 30_000m,
        });
        entrada.StatusCode.Should().Be(HttpStatusCode.Created);

        var reporte = await cliente.GetFromJsonAsync<Reporte>(Ruta);
        var linea = reporte!.Lineas.Single(l => l.ProductoId == productoId);

        linea.CostoPromedio.Should().Be(30_000m);
        linea.ValorInventario.Should().Be(300_000m, "10 unidades a 30.000 de costo");
    }

    [Fact]
    public async Task El_costo_promedio_pondera_entradas_a_precios_distintos()
    {
        var cliente = fabrica.CrearCliente("Admin");
        var (productoId, _) = await CrearProductoAsync(cliente, precioVenta: 50_000m, stockInicial: 0, minima: 1);

        await cliente.PostAsJsonAsync($"/api/merchandising/productos/{productoId}/entradas", new
        {
            cantidad = 10, fecha = DateTime.UtcNow, observaciones = "Lote barato", costoUnitario = 20_000m,
        });

        await cliente.PostAsJsonAsync($"/api/merchandising/productos/{productoId}/entradas", new
        {
            cantidad = 10, fecha = DateTime.UtcNow, observaciones = "Lote caro", costoUnitario = 40_000m,
        });

        var reporte = await cliente.GetFromJsonAsync<Reporte>(Ruta);
        var linea = reporte!.Lineas.Single(l => l.ProductoId == productoId);

        // (10 x 20.000 + 10 x 40.000) / 20 = 30.000
        linea.CostoPromedio.Should().Be(30_000m);
    }

    [Fact]
    public async Task Una_venta_produce_utilidad_y_margen()
    {
        var cliente = fabrica.CrearCliente("Admin");
        var (productoId, bancoId) = await CrearProductoAsync(cliente, precioVenta: 50_000m, stockInicial: 0, minima: 1);

        await cliente.PostAsJsonAsync($"/api/merchandising/productos/{productoId}/entradas", new
        {
            cantidad = 10, fecha = DateTime.UtcNow, observaciones = "Compra", costoUnitario = 30_000m,
        });

        var venta = await cliente.PostAsJsonAsync($"/api/merchandising/productos/{productoId}/ventas", new
        {
            cantidad = 4,
            bancoId,
            concepto = "Venta en rodada",
            medioPago = 1,
        });
        venta.StatusCode.Should().Be(HttpStatusCode.Created);

        var reporte = await cliente.GetFromJsonAsync<Reporte>(Ruta);
        var linea = reporte!.Lineas.Single(l => l.ProductoId == productoId);

        linea.UnidadesVendidas.Should().Be(4);
        linea.IngresoVentas.Should().Be(200_000m);
        linea.CostoVentas.Should().Be(120_000m);
        linea.Utilidad.Should().Be(80_000m);
        linea.MargenPorcentaje.Should().Be(40m);
    }

    [Fact]
    public async Task El_reporte_marca_los_productos_bajo_el_minimo()
    {
        var cliente = fabrica.CrearCliente("Admin");
        var (productoId, _) = await CrearProductoAsync(cliente, precioVenta: 10_000m, stockInicial: 2, minima: 5);

        var reporte = await cliente.GetFromJsonAsync<Reporte>(Ruta);
        var linea = reporte!.Lineas.Single(l => l.ProductoId == productoId);

        // Es lo primero que necesita ver quien repone.
        linea.BajoMinimo.Should().BeTrue();
        reporte.ProductosBajoMinimo.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Un_producto_sin_costo_registrado_no_inventa_utilidad()
    {
        var cliente = fabrica.CrearCliente("Admin");
        var (productoId, _) = await CrearProductoAsync(cliente, precioVenta: 10_000m, stockInicial: 5, minima: 1);

        var reporte = await cliente.GetFromJsonAsync<Reporte>(Ruta);
        var linea = reporte!.Lineas.Single(l => l.ProductoId == productoId);

        // Sin entradas con costo el promedio es cero, y el reporte lo muestra
        // asi en vez de suponer un margen que nadie declaro.
        linea.CostoPromedio.Should().Be(0m);
        linea.ValorInventario.Should().Be(0m);
    }

    private sealed record LineaReporte(
        Guid ProductoId,
        string Nombre,
        int CantidadEnStock,
        bool BajoMinimo,
        decimal CostoPromedio,
        decimal ValorInventario,
        int UnidadesVendidas,
        decimal IngresoVentas,
        decimal CostoVentas,
        decimal Utilidad,
        decimal MargenPorcentaje);

    private sealed record Reporte(
        decimal ValorTotalInventario,
        int TotalUnidadesVendidas,
        decimal TotalIngresoVentas,
        decimal TotalCostoVentas,
        decimal UtilidadTotal,
        int ProductosBajoMinimo,
        List<LineaReporte> Lineas);

    private sealed record RespuestaId(Guid Id);
}
