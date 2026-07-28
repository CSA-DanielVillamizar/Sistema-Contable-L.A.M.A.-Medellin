using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Seeders;

public static class ConsecutivoComprobanteSeeder
{
    /// <summary>
    /// Crea el contador que falte, sin tocar los existentes: es idempotente y
    /// seguro de ejecutar sobre una base que ya viene numerando.
    /// </summary>
    public static async Task SeedConsecutivosComprobanteAsync(this LamaDbContext context)
    {
        var existentes = await context.ConsecutivosComprobante
            .Select(c => c.TipoComprobante)
            .ToListAsync();

        var faltantes = Enum.GetValues<TipoComprobante>()
            .Where(tipo => !existentes.Contains(tipo))
            .Select(tipo => new ConsecutivoComprobante(tipo))
            .ToList();

        if (faltantes.Count == 0)
        {
            return;
        }

        await context.ConsecutivosComprobante.AddRangeAsync(faltantes);
        await context.SaveChangesAsync();
    }
}
