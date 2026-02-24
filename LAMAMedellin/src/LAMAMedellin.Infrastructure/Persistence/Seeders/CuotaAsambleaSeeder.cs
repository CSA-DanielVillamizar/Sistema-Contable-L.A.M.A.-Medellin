using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Seeders;

public static class CuotaAsambleaSeeder
{
    public static async Task SeedCuotasAsambleaAsync(this LamaDbContext context)
    {
        if (await context.Set<CuotaAsamblea>().AnyAsync())
        {
            return;
        }

        var cuotas = new List<CuotaAsamblea>
        {
            // Cuota histórica (Jan 2026 and earlier): 20,000 COP
            new(2026, 20000M, 1, "Acta Asamblea Diciembre 2025 (Cuota Histórica)"),
            
            // Cuota nueva (desde Feb 2026): 25,000 COP
            new(2026, 25000M, 2, "Acta Asamblea Enero 2026")
        };

        await context.Set<CuotaAsamblea>().AddRangeAsync(cuotas);
        await context.SaveChangesAsync();
    }
}

