using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LAMAMedellin.Infrastructure.Persistence;

/// <summary>
/// Factory para herramientas de diseño de EF Core (migraciones, scaffolding).
/// Usa una cadena de conexión SQL Server local sin credenciales Azure para permitir
/// la ejecución de `dotnet ef migrations add` en entornos de desarrollo/CI sin Azure.
/// </summary>
public sealed class LamaDbContextDesignTimeFactory : IDesignTimeDbContextFactory<LamaDbContext>
{
    public LamaDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("DESIGN_TIME_CONNECTION_STRING")
            ?? "Server=(localdb)\\mssqllocaldb;Database=LAMAMedellin_DesignTime;Trusted_Connection=True;";

        var optionsBuilder = new DbContextOptionsBuilder<LamaDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new LamaDbContext(optionsBuilder.Options);
    }
}
