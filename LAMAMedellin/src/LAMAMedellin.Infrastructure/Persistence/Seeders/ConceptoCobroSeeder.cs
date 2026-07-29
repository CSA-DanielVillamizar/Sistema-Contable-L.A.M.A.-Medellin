using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Seeders;

public static class ConceptoCobroSeeder
{
    /// <summary>
    /// Nombre del concepto con el que se genera la cuota mensual. La generacion
    /// lo busca por este nombre, de modo que no puede cambiarse sin migrar.
    /// </summary>
    public const string NombreCuotaMensual = "Cuota de sostenimiento mensual";

    /// <summary>
    /// Siembra el concepto de cuota mensual. El valor no se define aqui: sale de
    /// la cuota vigente aprobada en asamblea para cada periodo, asi que el
    /// ValorCOP del concepto es solo referencia.
    /// </summary>
    public static async Task SeedConceptosCobroAsync(this LamaDbContext context)
    {
        if (await context.ConceptosCobro.AnyAsync(c => c.Nombre == NombreCuotaMensual))
        {
            return;
        }

        var cuentaCuotas = await context.CuentasContables
            .FirstOrDefaultAsync(c => c.Codigo == "410510");

        if (cuentaCuotas is null)
        {
            return;
        }

        await context.ConceptosCobro.AddAsync(new ConceptoCobro(
            nombre: NombreCuotaMensual,
            valorCop: 1m,
            periodicidadMensual: 1,
            cuentaContableIngresoId: cuentaCuotas.Id));

        await context.SaveChangesAsync();
    }
}
