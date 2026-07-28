using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Seeders;

public static class CentroCostoSeeder
{
    /// <summary>
    /// Siembra los centros de costo base (historia 0-5 del backlog:
    /// CAPITULO, FUNDACION, PROYECTO, EVENTO).
    ///
    /// Sin al menos uno el sistema no puede registrar NINGUN movimiento
    /// contable: CentroCostoId es obligatorio en asientos, ingresos, egresos y
    /// transacciones. Como no existia siembra ni endpoint de creacion, una base
    /// nueva quedaba inutilizable; produccion solo funciona porque los centros
    /// se insertaron a mano por SQL.
    ///
    /// Es idempotente: solo crea los tipos que falten.
    /// </summary>
    public static async Task SeedCentrosCostoAsync(this LamaDbContext context)
    {
        var tiposExistentes = await context.CentrosCosto
            .Select(c => c.Tipo)
            .ToListAsync();

        CentroCosto[] base_ =
        [
            new("Capitulo Medellin", TipoCentroCosto.Capitulo),
            new("Fundacion", TipoCentroCosto.Fundacion),
            new("Proyectos sociales", TipoCentroCosto.Proyecto),
            new("Eventos y rodadas", TipoCentroCosto.Evento),
        ];

        var faltantes = base_
            .Where(centro => !tiposExistentes.Contains(centro.Tipo))
            .ToList();

        if (faltantes.Count == 0)
        {
            return;
        }

        await context.CentrosCosto.AddRangeAsync(faltantes);
        await context.SaveChangesAsync();
    }
}
