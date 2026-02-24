using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Seeders;

/// <summary>
/// Precarga el Catálogo de Cuentas (PUC) adaptado para la Fundación L.A.M.A. Medellín.
/// Marco normativo: NIIF para Microempresas (Grupo III), ESAL vigilada por Gobernación de Antioquia.
/// Responsabilidades tributarias: 05-Renta, 07-Retención, 42-Contabilidad, 48-IVA, 14-Exógena.
/// </summary>
public static class CuentaContableSeeder
{
    public static async Task SeedCuentasContablesAsync(this LamaDbContext context)
    {
        var existenCuentas = await context.CuentasContables.AnyAsync();
        if (existenCuentas)
        {
            return;
        }

        // ──────────────────────────────────────────────────────────────────────
        // Definición del catálogo completo (Patrimonio + Ingresos + Gastos + Costos)
        // Formato: (Codigo, Descripcion, Naturaleza, PermiteMovimiento, ExigeTercero)
        // Activos (1) y Pasivos (2) se agregarán en iteración posterior.
        // ──────────────────────────────────────────────────────────────────────
        var definiciones = new (string Codigo, string Descripcion, NaturalezaCuenta Naturaleza, bool PermiteMovimiento, bool ExigeTercero)[]
        {
            // ── Clase 3: Patrimonio Institucional ─────────────────────────────
            ("3",      "PATRIMONIO INSTITUCIONAL",                NaturalezaCuenta.Credito, false, false),
            ("31",     "Fondo Social",                            NaturalezaCuenta.Credito, false, false),
            ("3105",   "Aportes de Fundadores",                   NaturalezaCuenta.Credito, false, false),
            ("310505", "Aportes en Dinero",                       NaturalezaCuenta.Credito, true,  true),
            ("310510", "Aportes en Especie",                      NaturalezaCuenta.Credito, true,  true),
            ("3115",   "Fondo de Destinación Específica",         NaturalezaCuenta.Credito, false, false),
            ("311505", "Reserva para proyectos misionales",       NaturalezaCuenta.Credito, true,  false),
            ("32",     "Resultados del Ejercicio (No Utilidades)", NaturalezaCuenta.Credito, false, false),
            ("3205",   "Excedente del Ejercicio",                 NaturalezaCuenta.Credito, true,  false),
            ("3210",   "Déficit del Ejercicio",                   NaturalezaCuenta.Debito,  true,  false),

            // ── Clase 4: Ingresos ─────────────────────────────────────────────
            ("4",      "INGRESOS",                                NaturalezaCuenta.Credito, false, false),
            ("41",     "Ingresos de Actividades Ordinarias",      NaturalezaCuenta.Credito, false, false),
            ("4105",   "Aportes y Cuotas de Sostenimiento",       NaturalezaCuenta.Credito, false, false),
            ("410505", "Cuotas de Afiliación (Nuevos)",           NaturalezaCuenta.Credito, true,  true),
            ("410510", "Cuotas de Sostenimiento (Mensualidad)",   NaturalezaCuenta.Credito, true,  true),
            ("4110",   "Ingresos por Eventos y Actividades",      NaturalezaCuenta.Credito, false, false),
            ("411005", "Inscripciones a Rodadas y Eventos",       NaturalezaCuenta.Credito, true,  true),
            ("411010", "Venta de Merchandising (Parches, etc.)",  NaturalezaCuenta.Credito, true,  false),
            ("4115",   "Donaciones Recibidas",                    NaturalezaCuenta.Credito, false, false),
            ("411505", "Donaciones No Condicionadas (Libres)",    NaturalezaCuenta.Credito, true,  true),
            ("411510", "Donaciones Condicionadas (Proyectos)",    NaturalezaCuenta.Credito, true,  true),

            // ── Clase 5: Gastos Administrativos ──────────────────────────────
            ("5",      "GASTOS ADMINISTRATIVOS",                  NaturalezaCuenta.Debito,  false, false),
            ("51",     "Operación y Administración",              NaturalezaCuenta.Debito,  false, false),
            ("5105",   "Gastos de Representación",                NaturalezaCuenta.Debito,  false, false),
            ("510505", "Reuniones de Junta Directiva",            NaturalezaCuenta.Debito,  true,  false),
            ("5110",   "Honorarios y Servicios",                  NaturalezaCuenta.Debito,  false, false),
            ("511005", "Honorarios Contables y Legales",          NaturalezaCuenta.Debito,  true,  true),

            // ── Clase 6: Costos de Proyectos Misionales ───────────────────────
            ("6",      "COSTOS DE PROYECTOS MISIONALES",          NaturalezaCuenta.Debito,  false, false),
            ("61",     "Costos de Eventos y Rodadas",             NaturalezaCuenta.Debito,  false, false),
            ("6105",   "Logística de Eventos",                    NaturalezaCuenta.Debito,  false, false),
            ("610505", "Alquiler de Espacios / Permisos",         NaturalezaCuenta.Debito,  true,  true),
            ("610510", "Alimentación y Refrigerios",              NaturalezaCuenta.Debito,  true,  true),
            ("610515", "Reconocimientos y Trofeos",               NaturalezaCuenta.Debito,  true,  true),
        };

        // Instanciar las entidades
        var cuentas = definiciones
            .Select(d => new CuentaContable(d.Codigo, d.Descripcion, d.Naturaleza, d.PermiteMovimiento, d.ExigeTercero))
            .ToList();

        // Construir índice para resolución de padres
        var indicePorCodigo = cuentas.ToDictionary(c => c.Codigo);

        // Segunda pasada: establecer relación padre → hijo
        foreach (var cuenta in cuentas)
        {
            var codigoPadre = CuentaContable.ResolverCodigoPadre(cuenta.Codigo);
            if (codigoPadre is not null && indicePorCodigo.TryGetValue(codigoPadre, out var padre))
            {
                cuenta.EstablecerPadre(padre.Id);
            }
        }

        await context.CuentasContables.AddRangeAsync(cuentas);
        await context.SaveChangesAsync();
    }
}
