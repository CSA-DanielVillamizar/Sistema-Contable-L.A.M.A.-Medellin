using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Seeders;

public static class BancoSeeder
{
    /// <summary>
    /// Siembra la cuenta bancaria de la fundacion. Toda la tesoreria pasa por
    /// aqui: la operacion es 100% bancarizada, sin manejo de efectivo.
    ///
    /// Se apoya en la cuenta contable 111005 (Bancos - Moneda Nacional), que
    /// siembra el catalogo del PUC.
    /// </summary>
    public static async Task SeedBancoAsync(this LamaDbContext context)
    {
        if (await context.Bancos.AnyAsync())
        {
            return;
        }

        var cuentaBanco = await context.CuentasContables
            .FirstOrDefaultAsync(x => x.Codigo == "111005");

        if (cuentaBanco is null)
        {
            return;
        }

        await context.Bancos.AddAsync(new Banco(
            nombre: "Bancolombia Ahorros",
            numeroCuenta: "23000013774",
            saldoActual: 0m,
            cuentaContableId: cuentaBanco.Id));

        await context.SaveChangesAsync();
    }
}
