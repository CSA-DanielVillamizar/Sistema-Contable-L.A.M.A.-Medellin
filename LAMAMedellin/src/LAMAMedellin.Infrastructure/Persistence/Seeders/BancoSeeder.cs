using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Seeders;

public static class BancoSeeder
{
    public static async Task SeedBancoAsync(this LamaDbContext context)
    {
        var existeBanco = await context.Bancos.AnyAsync();
        if (existeBanco)
        {
            return;
        }

        context.Bancos.Add(new Banco("CTA-PRINCIPAL-LAMA", 0m));
        await context.SaveChangesAsync();
    }
}
