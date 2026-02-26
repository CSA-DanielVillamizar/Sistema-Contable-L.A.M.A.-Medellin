using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using LAMAMedellin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Seeders;

public static class CuentaContableSeeder
{
    private sealed record CuentaSeed(
        string Codigo,
        string Descripcion,
        NaturalezaCuenta Naturaleza,
        bool PermiteMovimiento,
        bool ExigeTercero);

    public static async Task SeedCuentasContablesAsync(this LamaDbContext context)
    {
        var cuentasSeed = ObtenerCuentasCore();
        var codigos = cuentasSeed.Select(x => x.Codigo).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var existentes = await context.CuentasContables
            .Where(x => codigos.Contains(x.Codigo))
            .ToListAsync();

        if (existentes.Count == cuentasSeed.Count)
        {
            return;
        }

        var cuentasPorCodigo = existentes.ToDictionary(x => x.Codigo, StringComparer.OrdinalIgnoreCase);

        foreach (var seed in cuentasSeed.OrderBy(x => x.Codigo.Length).ThenBy(x => x.Codigo, StringComparer.Ordinal))
        {
            if (cuentasPorCodigo.ContainsKey(seed.Codigo))
            {
                continue;
            }

            Guid? cuentaPadreId = null;
            var codigoPadre = DeterminarCodigoPadre(seed.Codigo);

            if (!string.IsNullOrWhiteSpace(codigoPadre))
            {
                if (!cuentasPorCodigo.TryGetValue(codigoPadre, out var cuentaPadre))
                {
                    throw new InvalidOperationException($"No se encontró la cuenta padre '{codigoPadre}' para la cuenta '{seed.Codigo}'.");
                }

                cuentaPadreId = cuentaPadre.Id;
            }

            var nuevaCuenta = new CuentaContable(
                seed.Codigo,
                seed.Descripcion,
                seed.Naturaleza,
                seed.PermiteMovimiento,
                seed.ExigeTercero,
                cuentaPadreId);

            await context.CuentasContables.AddAsync(nuevaCuenta);
            cuentasPorCodigo[nuevaCuenta.Codigo] = nuevaCuenta;
        }

        await context.SaveChangesAsync();
    }

    private static string? DeterminarCodigoPadre(string codigo)
    {
        if (codigo.Length <= 1)
        {
            return null;
        }

        if (codigo.Length == 2)
        {
            return codigo[..1];
        }

        if (codigo.Length == 4)
        {
            return codigo[..2];
        }

        if (codigo.Length == 6)
        {
            return codigo[..4];
        }

        return codigo[..6];
    }

    private static List<CuentaSeed> ObtenerCuentasCore()
    {
        return
        [
            new("3", "PATRIMONIO INSTITUCIONAL", NaturalezaCuenta.Credito, false, false),
            new("31", "Fondo Social", NaturalezaCuenta.Credito, false, false),
            new("3105", "Aportes de Fundadores", NaturalezaCuenta.Credito, false, false),
            new("310505", "Aportes en Dinero", NaturalezaCuenta.Credito, true, true),
            new("310510", "Aportes en Especie", NaturalezaCuenta.Credito, true, true),
            new("3115", "Fondo de Destinación Específica", NaturalezaCuenta.Credito, false, false),
            new("311505", "Reserva para proyectos misionales", NaturalezaCuenta.Credito, true, false),
            new("32", "Resultados del Ejercicio (No Utilidades)", NaturalezaCuenta.Credito, false, false),
            new("3205", "Excedente del Ejercicio", NaturalezaCuenta.Credito, true, false),
            new("3210", "Déficit del Ejercicio", NaturalezaCuenta.Debito, true, false),

            new("4", "INGRESOS", NaturalezaCuenta.Credito, false, false),
            new("41", "Ingresos de Actividades Ordinarias", NaturalezaCuenta.Credito, false, false),
            new("4105", "Aportes y Cuotas de Sostenimiento", NaturalezaCuenta.Credito, false, false),
            new("410505", "Cuotas de Afiliación (Nuevos)", NaturalezaCuenta.Credito, true, true),
            new("410510", "Cuotas de Sostenimiento (Mensualidad)", NaturalezaCuenta.Credito, true, true),
            new("4110", "Ingresos por Eventos y Actividades", NaturalezaCuenta.Credito, false, false),
            new("411005", "Inscripciones a Rodadas y Eventos", NaturalezaCuenta.Credito, true, true),
            new("411010", "Venta de Merchandising (Parches, etc.)", NaturalezaCuenta.Credito, true, false),
            new("4115", "Donaciones Recibidas", NaturalezaCuenta.Credito, false, false),
            new("411505", "Donaciones No Condicionadas (Libres)", NaturalezaCuenta.Credito, true, true),
            new("411510", "Donaciones Condicionadas (Proyectos)", NaturalezaCuenta.Credito, true, true),

            new("5", "GASTOS ADMINISTRATIVOS", NaturalezaCuenta.Debito, false, false),
            new("51", "Operación y Administración", NaturalezaCuenta.Debito, false, false),
            new("5105", "Gastos de Representación", NaturalezaCuenta.Debito, false, false),
            new("510505", "Reuniones de Junta Directiva", NaturalezaCuenta.Debito, true, false),
            new("5110", "Honorarios y Servicios", NaturalezaCuenta.Debito, false, false),
            new("511005", "Honorarios Contables y Legales", NaturalezaCuenta.Debito, true, true),

            new("6", "COSTOS DE PROYECTOS MISIONALES", NaturalezaCuenta.Debito, false, false),
            new("61", "Costos de Eventos y Rodadas", NaturalezaCuenta.Debito, false, false),
            new("6105", "Logística de Eventos", NaturalezaCuenta.Debito, false, false),
            new("610505", "Alquiler de Espacios / Permisos", NaturalezaCuenta.Debito, true, true),
            new("610510", "Alimentación y Refrigerios", NaturalezaCuenta.Debito, true, true),
            new("610515", "Reconocimientos y Trofeos", NaturalezaCuenta.Debito, true, true),
        ];
    }
}
