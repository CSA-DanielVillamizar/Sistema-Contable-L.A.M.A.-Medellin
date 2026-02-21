using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Infrastructure.Persistence;
using LAMAMedellin.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LAMAMedellin.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada.");

        services.AddDbContext<LamaDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ITransaccionRepository, TransaccionRepository>();
        services.AddScoped<IBancoRepository, BancoRepository>();
        services.AddScoped<ICentroCostoRepository, CentroCostoRepository>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddInfrastructureServices(configuration);
    }
}
