using LAMAMedellin.Infrastructure.Persistence.Seeders;

namespace LAMAMedellin.Infrastructure.Persistence;

public static class LamaDbContextSeed
{
    public static async Task SeedAsync(this LamaDbContext context)
    {
        await context.SeedMiembrosAsync();
    }
}
