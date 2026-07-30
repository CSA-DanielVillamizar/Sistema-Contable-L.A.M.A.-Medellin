using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using LAMAMedellin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Seeders;

public static class MapeoContableSeeder
{
    /// <summary>
    /// Mapeo inicial de operaciones a cuentas (historia 1-2).
    ///
    /// Se siembra con los valores que el contador usaria de todos modos, para
    /// que el sistema arranque operativo. Son un punto de partida, no una
    /// decision cerrada: la pantalla de mapeo permite cambiarlos y cada cambio
    /// queda auditado.
    ///
    /// Es incremental: solo agrega las operaciones que aun no tienen cuenta, de
    /// modo que no pisa lo que el contador ya haya ajustado.
    /// </summary>
    private static readonly (TipoOperacionContable Operacion, string Codigo)[] Predeterminados =
    [
        (TipoOperacionContable.IngresoCuotas, "410510"),
        (TipoOperacionContable.IngresoDonaciones, "411505"),
        (TipoOperacionContable.IngresoMerchandising, "411010"),
        (TipoOperacionContable.IngresoDiferenciaCambio, "421805"),
        (TipoOperacionContable.GastoDiferenciaCambio, "530535"),
        (TipoOperacionContable.GastoAdministrativo, "519520"),
        (TipoOperacionContable.GastoOperativo, "513015"),
        (TipoOperacionContable.GastoEventos, "519520"),
        (TipoOperacionContable.GastoProyectos, "519520"),
        (TipoOperacionContable.GastoBancario, "530525"),
        (TipoOperacionContable.CompraInventario, "413005"),
    ];

    public static async Task SeedMapeosContablesAsync(this LamaDbContext context)
    {
        var existentes = await context.MapeosContables
            .Select(m => m.TipoOperacion)
            .ToListAsync();

        var faltantes = Predeterminados.Where(p => !existentes.Contains(p.Operacion)).ToList();

        if (faltantes.Count == 0)
        {
            return;
        }

        var codigos = faltantes.Select(f => f.Codigo).Distinct().ToList();

        var cuentas = await context.CuentasContables
            .Where(c => codigos.Contains(c.Codigo))
            .ToDictionaryAsync(c => c.Codigo, c => c.Id);

        foreach (var (operacion, codigo) in faltantes)
        {
            // Si la cuenta no esta en el catalogo se omite en silencio: sembrar
            // un mapeo apuntando a nada seria peor que dejarlo sin configurar,
            // porque la pantalla ya marca lo pendiente.
            if (!cuentas.TryGetValue(codigo, out var cuentaId))
            {
                continue;
            }

            await context.MapeosContables.AddAsync(new MapeoContable(operacion, cuentaId));
        }

        await context.SaveChangesAsync();
    }
}
