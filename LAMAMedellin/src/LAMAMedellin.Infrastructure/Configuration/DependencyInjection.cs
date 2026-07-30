using Azure.Core;
using Azure.Identity;
using LAMAMedellin.Application.Common.Interfaces.Services;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Infrastructure.Documents;
using LAMAMedellin.Infrastructure.Persistence;
using LAMAMedellin.Infrastructure.Persistence.Repositories;
using LAMAMedellin.Infrastructure.Storage;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LAMAMedellin.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada.");

        services.AddDbContext<LamaDbContext>(options =>
        {
            if (environment.IsDevelopment() && RequiereTokenEntraManual(connectionString))
            {
                // Development contra Azure SQL: la cadena no trae credenciales propias,
                // asi que hay que obtener un token Entra a mano.
                // Si la cadena ya declara autenticacion (SQL Server local en Docker o
                // Authentication=Active Directory *), se omite este bloque y resuelve SqlClient.
                var tokenValue = Environment.GetEnvironmentVariable("SQL_ACCESS_TOKEN");

                if (string.IsNullOrWhiteSpace(tokenValue))
                {
                    var tenantId = configuration["AzureAd:TenantId"];

                    var credential = new ChainedTokenCredential(
                        new AzureCliCredential(new AzureCliCredentialOptions
                        {
                            TenantId = tenantId,
                            ProcessTimeout = TimeSpan.FromSeconds(90)
                        }),
                        new AzureDeveloperCliCredential(new AzureDeveloperCliCredentialOptions
                        {
                            TenantId = tenantId
                        }),
                        new AzurePowerShellCredential(new AzurePowerShellCredentialOptions
                        {
                            TenantId = tenantId
                        }),
                        new DefaultAzureCredential(new DefaultAzureCredentialOptions
                        {
                            TenantId = tenantId,
                            ExcludeManagedIdentityCredential = true,
                            ExcludeEnvironmentCredential = true,
                            ExcludeSharedTokenCacheCredential = true,
                            ExcludeVisualStudioCredential = true,
                            ExcludeVisualStudioCodeCredential = true,
                            ExcludeInteractiveBrowserCredential = true,
                            ExcludeWorkloadIdentityCredential = true
                        }));

                    var requestContext = new TokenRequestContext(new[] { "https://database.windows.net/.default" });
                    Exception? ultimoError = null;

                    for (var intento = 1; intento <= 3; intento++)
                    {
                        try
                        {
                            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(90));
                            tokenValue = credential.GetToken(requestContext, cts.Token).Token;
                            break;
                        }
                        catch (Exception ex)
                        {
                            ultimoError = ex;
                            if (intento < 3)
                            {
                                Thread.Sleep(TimeSpan.FromSeconds(2));
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(tokenValue) && ultimoError is not null)
                    {
                        throw new InvalidOperationException(
                            "No fue posible obtener un token Entra ID para Azure SQL en Development.",
                            ultimoError);
                    }
                }

                var sqlConnection = new SqlConnection(connectionString)
                {
                    AccessToken = tokenValue
                };

                options.UseSqlServer(sqlConnection, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
            }
            else
            {
                // Production: Usar DefaultAzureCredential
                // En Azure App Service, esto automáticamente usará Managed Identity (System Assigned)
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
            }
        });

        services.AddScoped<ITransaccionRepository, TransaccionRepository>();
        services.AddScoped<IIngresoRepository, IngresoRepository>();
        services.AddScoped<IEgresoRepository, EgresoRepository>();
        services.AddScoped<IBancoRepository, BancoRepository>();
        services.AddScoped<ICentroCostoRepository, CentroCostoRepository>();
        services.AddScoped<IMiembroRepository, MiembroRepository>();
        services.AddScoped<ICuotaAsambleaRepository, CuotaAsambleaRepository>();
        services.AddScoped<ITarifaCuotaRepository, TarifaCuotaRepository>();
        services.AddScoped<ICuentaPorCobrarRepository, CuentaPorCobrarRepository>();
        services.AddScoped<ICuentaPorPagarRepository, CuentaPorPagarRepository>();
        services.AddScoped<IMapeoContableRepository, MapeoContableRepository>();
        services.AddScoped<ISolicitudAnulacionRepository, SolicitudAnulacionRepository>();
        services.AddScoped<ICampanaDonacionRepository, CampanaDonacionRepository>();
        services.AddScoped<IActividadProyectoRepository, ActividadProyectoRepository>();
        services.AddScoped<IConceptoCobroRepository, ConceptoCobroRepository>();
        services.AddScoped<ICuentaContableRepository, CuentaContableRepository>();
        services.AddScoped<IDonanteRepository, DonanteRepository>();
        services.AddScoped<IDonacionRepository, DonacionRepository>();
        services.AddScoped<ICertificadoDonacionService, CertificadoDonacionService>();
        services.AddSingleton<IConfiguracionSeguridad, ConfiguracionSeguridad>();
        services.AddScoped<IReciboService, ReciboService>();
        services.AddScoped<ITransactionManager, TransactionManager>();
        services.AddScoped<IGeneradorConsecutivos, GeneradorConsecutivos>();
        services.AddScoped<IComprobanteRepository, ComprobanteRepository>();
        services.AddScoped<IPeriodoContableRepository, PeriodoContableRepository>();
        services.AddScoped<ILibrosContablesRepository, LibrosContablesRepository>();
        services.AddScoped<IProyectoSocialRepository, ProyectoSocialRepository>();
        services.AddScoped<IBeneficiarioRepository, BeneficiarioRepository>();
        services.AddScoped<ITributarioRepository, TributarioRepository>();
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IMovimientoInventarioRepository, MovimientoInventarioRepository>();
        services.AddScoped<IEventoRepository, EventoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddSingleton<IFileStorageService, AzureBlobStorageService>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        return services.AddInfrastructureServices(configuration, environment);
    }

    /// <summary>
    /// Indica si hay que obtener un token Entra ID a mano para abrir la conexion.
    /// Solo hace falta cuando la cadena no declara como autenticarse: si trae
    /// credenciales SQL (SQL Server local en Docker) o un modo Authentication
    /// explicito, SqlClient lo resuelve por su cuenta.
    /// </summary>
    private static bool RequiereTokenEntraManual(string connectionString)
    {
        SqlConnectionStringBuilder builder;

        try
        {
            builder = new SqlConnectionStringBuilder(connectionString);
        }
        catch (ArgumentException)
        {
            // Cadena malformada: que falle mas adelante con el error real de SqlClient.
            return false;
        }

        var tieneCredencialesSql =
            !string.IsNullOrWhiteSpace(builder.UserID) && !string.IsNullOrWhiteSpace(builder.Password);

        var declaraAutenticacion = builder.Authentication != SqlAuthenticationMethod.NotSpecified;

        return !tieneCredencialesSql && !declaraAutenticacion;
    }
}
