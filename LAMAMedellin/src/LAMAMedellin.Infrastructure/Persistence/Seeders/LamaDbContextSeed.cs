using LAMAMedellin.Infrastructure.Seeders;
using LAMAMedellin.Infrastructure.Persistence.Seeders;

namespace LAMAMedellin.Infrastructure.Persistence;

public static class LamaDbContextSeed
{
    public static async Task SeedAsync(this LamaDbContext context)
    {
        await context.SeedConsecutivosComprobanteAsync();
        await context.SeedCuentasContablesAsync();
        await context.SeedCentrosCostoAsync();
        await context.SeedBancoAsync();
        await context.SeedCuotasAsambleaAsync();
        await context.SeedTarifasCuotaAsync();
        await context.SeedMiembrosAsync();
    }
}
