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
    /// La generacion de renovacion anual lo busca por este nombre (historia
    /// fx-02), igual que la cuota mensual busca la suya.
    /// </summary>
    public const string NombreRenovacionAnual = "Renovacion membresia internacional";

    /// <summary>
    /// Siembra el concepto de cuota mensual. El valor no se define aqui: sale de
    /// la cuota vigente aprobada en asamblea para cada periodo, asi que el
    /// ValorCOP del concepto es solo referencia.
    /// </summary>
    public static async Task SeedConceptosCobroAsync(this LamaDbContext context)
    {
        if (!await context.ConceptosCobro.AnyAsync(c => c.Nombre == NombreCuotaMensual))
        {
            var cuentaCuotas = await context.CuentasContables
                .FirstOrDefaultAsync(c => c.Codigo == "410510");

            if (cuentaCuotas is not null)
            {
                await context.ConceptosCobro.AddAsync(new ConceptoCobro(
                    nombre: NombreCuotaMensual,
                    valorCop: 1m,
                    periodicidadMensual: 1,
                    cuentaContableIngresoId: cuentaCuotas.Id));
            }
        }

        if (!await context.ConceptosCobro.AnyAsync(c => c.Nombre == NombreRenovacionAnual))
        {
            // 281505: pasivo (dinero de terceros), no ingreso propio del
            // capitulo. El nombre del campo dice "Ingreso" pero solo es la
            // cuenta contable a acreditar; no exige que sea clase 4.
            var cuentaRenovacion = await context.CuentasContables
                .FirstOrDefaultAsync(c => c.Codigo == "281505");

            if (cuentaRenovacion is not null)
            {
                await context.ConceptosCobro.AddAsync(new ConceptoCobro(
                    nombre: NombreRenovacionAnual,
                    valorCop: 1m,
                    periodicidadMensual: 12,
                    cuentaContableIngresoId: cuentaRenovacion.Id));
            }
        }

        await context.SaveChangesAsync();
    }
}
