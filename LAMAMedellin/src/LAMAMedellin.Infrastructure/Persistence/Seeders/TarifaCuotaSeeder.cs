using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Seeders;

public static class TarifaCuotaSeeder
{
    public static async Task SeedTarifasCuotaAsync(this LamaDbContext context)
    {
        if (await context.Set<TarifaCuota>().AnyAsync())
        {
            return;
        }

        var cuotaBase = await context.Set<CuotaAsamblea>()
            .OrderByDescending(x => x.Anio)
            .ThenByDescending(x => x.MesInicioCobro)
            .Select(x => x.ValorMensualCOP)
            .FirstOrDefaultAsync();

        if (cuotaBase <= 0)
        {
            cuotaBase = 25000M;
        }

        var tarifas = Enum.GetValues<TipoAfiliacion>()
            .Select(tipo => new TarifaCuota(
                tipo,
                tipo.ExentoDeCuotaMensual() ? 0M : cuotaBase))
            .ToList();

        await context.Set<TarifaCuota>().AddRangeAsync(tarifas);
        await context.SaveChangesAsync();
    }
}
