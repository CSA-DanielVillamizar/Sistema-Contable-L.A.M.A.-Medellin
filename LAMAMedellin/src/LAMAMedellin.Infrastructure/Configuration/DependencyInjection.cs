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
                // Development: Usar Azure CLI Credential explícitamente
                // Esto usa tu sesión de `az login` que ya está funcionando
                var credential = new AzureCliCredential();
                var token = credential.GetToken(
                    new TokenRequestContext(new[] { "https://database.windows.net//.default" }),
                    default);

                var sqlConnection = new SqlConnection(connectionString)
                {
                    AccessToken = token.Token
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
