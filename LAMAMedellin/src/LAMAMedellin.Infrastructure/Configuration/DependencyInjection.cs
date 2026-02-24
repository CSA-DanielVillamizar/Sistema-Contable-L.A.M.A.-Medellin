using Azure.Core;
using Azure.Identity;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Infrastructure.Persistence;
using LAMAMedellin.Infrastructure.Persistence.Repositories;
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
            if (environment.IsDevelopment())
            {
                // Development: Token SQL desde variable de entorno (si existe)
                // Fallback: cadena de credenciales Entra (CLI, DevCLI, PowerShell, Default)
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

                options.UseSqlServer(sqlConnection);
            }
            else
            {
                // Production: Usar DefaultAzureCredential
                // En Azure App Service, esto automáticamente usará Managed Identity (System Assigned)
                options.UseSqlServer(connectionString);
            }
        });

        services.AddScoped<ITransaccionRepository, TransaccionRepository>();
        services.AddScoped<IBancoRepository, BancoRepository>();
        services.AddScoped<ICentroCostoRepository, CentroCostoRepository>();
        services.AddScoped<IMiembroRepository, MiembroRepository>();
        services.AddScoped<ICuotaAsambleaRepository, CuotaAsambleaRepository>();
        services.AddScoped<ICuentaPorCobrarRepository, CuentaPorCobrarRepository>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        return services.AddInfrastructureServices(configuration, environment);
    }
}
