using System.Security.Claims;
using System.Text.Encodings.Web;
using LAMAMedellin.Application.Common.Interfaces.Services;
using LAMAMedellin.Domain.Enums;
using LAMAMedellin.Infrastructure.Persistence;
using LAMAMedellin.Infrastructure.Seeders;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LAMAMedellin.API.Tests.Infraestructura;

/// <summary>
/// Levanta la API completa en memoria para poder ejercitarla por HTTP.
///
/// Hasta ahora todas las pruebas llamaban a un handler o a un controlador
/// directamente, de modo que nada cubria el trayecto real de una peticion:
/// enrutamiento, politica de autorizacion, binding del cuerpo, la tuberia de
/// validacion, el handler, EF y el manejador global de excepciones. Justamente
/// ahi es donde aparecieron los fallos que se vieron en pantalla (payloads que
/// el binding descartaba en silencio, un 422 sin mensaje, endpoints que no
/// existian).
///
/// La autenticacion de Entra se reemplaza por un esquema de prueba: no se
/// simula la emision del token, se da por autenticado y se declaran los roles
/// que la prueba necesita. La autorizacion real, la que decide si un rol basta,
/// sigue siendo la de produccion.
/// </summary>
public sealed class FabricaApiPruebas : WebApplicationFactory<Program>
{
    /// <summary>
    /// SQLite en memoria y no el proveedor InMemory de EF: el generador de
    /// consecutivos usa transacciones, que son metodos relacionales y el
    /// proveedor InMemory no implementa. Con SQLite las pruebas atraviesan la
    /// misma ruta relacional que produccion.
    ///
    /// La conexion se mantiene abierta durante toda la vida de la fabrica
    /// porque SQLite descarta la base en memoria al cerrar la ultima conexion.
    /// </summary>
    private readonly SqliteConnection conexion = new("DataSource=:memory:");

    /// <summary>
    /// Los roles viajan en una cabecera por peticion, no en estado compartido:
    /// xUnit ejecuta las clases de prueba en paralelo y un campo estatico hacia
    /// que una clase le cambiara los roles a otra a mitad de camino.
    /// </summary>
    public const string CabeceraRoles = "X-Roles-Prueba";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Cualquier entorno distinto de Development evita que Program aplique
        // migraciones contra SQL Server al arrancar. La siembra se hace aqui,
        // de forma explicita y contra la base en memoria.
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            QuitarRegistroDe<DbContextOptions<LamaDbContext>>(services);
            QuitarRegistroDe<LamaDbContext>(services);

            conexion.Open();
            services.AddDbContext<LamaDbContext>(options => options.UseSqlite(conexion));

            // El generador de consecutivos de produccion reserva el numero con
            // un UPDATE ... OUTPUT, que es T-SQL y SQLite no sabe interpretar.
            // Se reemplaza por uno equivalente en comportamiento observable
            // (numeros unicos y crecientes por tipo) para que estas pruebas
            // puedan cubrir todo lo que viene despues.
            //
            // La atomicidad real del generador, que es lo que evita huecos y
            // duplicados bajo concurrencia, no queda cubierta aqui: eso exige
            // SQL Server y necesita su propia prueba.
            QuitarRegistroDe<IGeneradorConsecutivos>(services);
            services.AddSingleton<IGeneradorConsecutivos, GeneradorConsecutivosPruebas>();

            services.AddAuthentication(EsquemaPruebas.Nombre)
                .AddScheme<AuthenticationSchemeOptions, ManejadorAutenticacionPruebas>(
                    EsquemaPruebas.Nombre, _ => { });

            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = EsquemaPruebas.Nombre;
                options.DefaultChallengeScheme = EsquemaPruebas.Nombre;
                options.DefaultScheme = EsquemaPruebas.Nombre;
            });
        });
    }

    /// <summary>
    /// Cliente autenticado con los roles indicados. Sin argumentos toma los tres
    /// roles, que es lo comodo para las pruebas que no verifican permisos.
    /// </summary>
    public HttpClient CrearCliente(params string[] roles)
    {
        var efectivos = roles.Length > 0 ? roles : ["Admin", "Tesorero", "Contador"];

        var cliente = CreateClient();
        cliente.DefaultRequestHeaders.Add(CabeceraRoles, string.Join(',', efectivos));

        return cliente;
    }

    /// <summary>Siembra el catalogo base (PUC, centros de costo, banco, conceptos).</summary>
    public async Task<LamaDbContext> PrepararBaseAsync()
    {
        var scope = Services.CreateScope();
        var contexto = scope.ServiceProvider.GetRequiredService<LamaDbContext>();

        await contexto.Database.EnsureCreatedAsync();
        await contexto.SeedAsync();

        return contexto;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            conexion.Dispose();
        }
    }

    private static void QuitarRegistroDe<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(T));

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }
    }
}

/// <summary>
/// Consecutivos para pruebas: unicos y crecientes por tipo de comprobante.
/// </summary>
internal sealed class GeneradorConsecutivosPruebas : IGeneradorConsecutivos
{
    private readonly Dictionary<TipoComprobante, int> contadores = [];
    private readonly object candado = new();

    public Task<string> SiguienteAsync(TipoComprobante tipoComprobante, CancellationToken cancellationToken = default)
    {
        lock (candado)
        {
            contadores.TryGetValue(tipoComprobante, out var actual);
            var siguiente = actual + 1;
            contadores[tipoComprobante] = siguiente;

            return Task.FromResult($"{tipoComprobante.ToString().ToUpperInvariant()[..2]}-{siguiente:D6}");
        }
    }
}

internal static class EsquemaPruebas
{
    public const string Nombre = "PruebasIntegracion";
}

internal sealed class ManejadorAutenticacionPruebas(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "11111111-1111-1111-1111-111111111111"),
            new("oid", "11111111-1111-1111-1111-111111111111"),
            new(ClaimTypes.Name, "prueba@lamamedellin.org"),
        };

        if (Context.Request.Headers.TryGetValue(FabricaApiPruebas.CabeceraRoles, out var cabecera))
        {
            claims.AddRange(cabecera
                .ToString()
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(rol => new Claim(ClaimTypes.Role, rol)));
        }

        var identidad = new ClaimsIdentity(claims, EsquemaPruebas.Nombre);
        var principal = new ClaimsPrincipal(identidad);
        var ticket = new AuthenticationTicket(principal, EsquemaPruebas.Nombre);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
