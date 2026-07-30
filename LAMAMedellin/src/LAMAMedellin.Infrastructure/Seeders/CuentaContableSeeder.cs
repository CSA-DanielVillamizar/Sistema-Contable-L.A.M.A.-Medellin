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
            new("1", "ACTIVO", NaturalezaCuenta.Debito, false, false),
            new("11", "Disponible", NaturalezaCuenta.Debito, false, false),
            new("1105", "Caja", NaturalezaCuenta.Debito, false, false),
            new("110505", "Caja General", NaturalezaCuenta.Debito, true, true),
            new("1110", "Bancos", NaturalezaCuenta.Debito, false, false),
            new("111005", "Moneda Nacional", NaturalezaCuenta.Debito, true, false),

            new("3", "PATRIMONIO INSTITUCIONAL", NaturalezaCuenta.Credito, false, false),
            new("31", "Fondo Social", NaturalezaCuenta.Credito, false, false),
            new("3105", "Aportes de Fundadores", NaturalezaCuenta.Credito, false, false),
            new("310505", "Aportes en Dinero", NaturalezaCuenta.Credito, true, true),
            new("310510", "Aportes en Especie", NaturalezaCuenta.Credito, true, true),
            new("37", "Resultados de Ejercicios Anteriores", NaturalezaCuenta.Credito, false, false),
            new("3705", "Saldos Iniciales", NaturalezaCuenta.Credito, false, false),
            new("370505", "Aportes Iniciales", NaturalezaCuenta.Credito, true, true),
            new("3115", "Fondo de Destinación Específica", NaturalezaCuenta.Credito, false, false),
            new("311505", "Reserva para proyectos misionales", NaturalezaCuenta.Credito, true, false),
            new("32", "Resultados del Ejercicio (No Utilidades)", NaturalezaCuenta.Credito, false, false),
            new("3205", "Excedente del Ejercicio", NaturalezaCuenta.Credito, true, false),
            new("3210", "Déficit del Ejercicio", NaturalezaCuenta.Debito, true, false),

            // Deudores. La cartera emite cuentas por cobrar desde el primer mes,
            // pero no existia ninguna cuenta donde registrarlas: el derecho de
            // cobro quedaba fuera del balance hasta que el miembro pagaba.
            new("13", "Deudores", NaturalezaCuenta.Debito, false, false),
            new("1305", "Cuotas por Cobrar a Miembros", NaturalezaCuenta.Debito, false, false),
            new("130505", "Cuotas de Sostenimiento por Cobrar", NaturalezaCuenta.Debito, true, true),
            new("130510", "Cuotas de Afiliación por Cobrar", NaturalezaCuenta.Debito, true, true),
            new("1380", "Deudores Varios", NaturalezaCuenta.Debito, false, false),
            new("138005", "Otros Deudores", NaturalezaCuenta.Debito, true, true),

            // Pasivo. El catalogo no tenia ni una sola cuenta de clase 2, asi
            // que ninguna obligacion podia registrarse y el balance no podia
            // cuadrar en cuanto hubiera algo pendiente de pagar.
            new("2", "PASIVO", NaturalezaCuenta.Credito, false, false),
            new("23", "Cuentas por Pagar", NaturalezaCuenta.Credito, false, false),
            new("2335", "Costos y Gastos por Pagar", NaturalezaCuenta.Credito, true, true),
            new("2380", "Acreedores Varios", NaturalezaCuenta.Credito, false, false),
            new("238005", "Otros Acreedores", NaturalezaCuenta.Credito, true, true),
            new("28", "Otros Pasivos", NaturalezaCuenta.Credito, false, false),
            new("2805", "Anticipos y Avances Recibidos", NaturalezaCuenta.Credito, false, false),
            new("280505", "Anticipos de Miembros", NaturalezaCuenta.Credito, true, true),

            // Ingresos recibidos para terceros. Es la figura que pidio el
            // cliente para la renovacion de membresia internacional de 20 USD
            // que se recauda en diciembre: el capitulo solo hace de puente
            // hacia el comite internacional, asi que ese dinero no es ingreso
            // propio (clase 4) sino una obligacion con un tercero.
            new("2815", "Ingresos Recibidos para Terceros", NaturalezaCuenta.Credito, false, false),
            new("281505", "Renovación Membresía Internacional L.A.M.A.", NaturalezaCuenta.Credito, true, true),

            new("4", "INGRESOS", NaturalezaCuenta.Credito, false, false),
            new("41", "Ingresos de Actividades Ordinarias", NaturalezaCuenta.Credito, false, false),
            new("4130", "Ventas", NaturalezaCuenta.Credito, false, false),
            new("413005", "Ventas de Mercancia", NaturalezaCuenta.Credito, true, true),
            new("4105", "Aportes y Cuotas de Sostenimiento", NaturalezaCuenta.Credito, false, false),
            new("410505", "Cuotas de Afiliación (Nuevos)", NaturalezaCuenta.Credito, true, true),
            new("410510", "Cuotas de Sostenimiento (Mensualidad)", NaturalezaCuenta.Credito, true, true),
            new("42", "Otros Ingresos", NaturalezaCuenta.Credito, false, false),
            new("4210", "Cuotas o Partes de Interes Social", NaturalezaCuenta.Credito, false, false),
            new("421005", "Cuotas Ordinarias", NaturalezaCuenta.Credito, true, true),
            new("4110", "Ingresos por Eventos y Actividades", NaturalezaCuenta.Credito, false, false),
            new("411005", "Inscripciones a Rodadas y Eventos", NaturalezaCuenta.Credito, true, true),
            new("411010", "Venta de Merchandising (Parches, etc.)", NaturalezaCuenta.Credito, true, false),
            new("4115", "Donaciones Recibidas", NaturalezaCuenta.Credito, false, false),
            new("411505", "Donaciones No Condicionadas (Libres)", NaturalezaCuenta.Credito, true, true),
            new("411510", "Donaciones Condicionadas (Proyectos)", NaturalezaCuenta.Credito, true, true),

            // Diferencia en cambio (historia 1-17). El PUC no las tenia, asi
            // que una obligacion en USD liquidada a una tasa distinta de la
            // reconocida no tenia donde registrar la ganancia ni la perdida.
            new("4218", "Diferencia en Cambio", NaturalezaCuenta.Credito, false, false),
            new("421805", "Ganancia por Diferencia en Cambio", NaturalezaCuenta.Credito, true, false),

            new("5", "GASTOS ADMINISTRATIVOS", NaturalezaCuenta.Debito, false, false),
            new("51", "Operación y Administración", NaturalezaCuenta.Debito, false, false),
            new("5130", "Servicios", NaturalezaCuenta.Debito, false, false),
            new("513015", "Transportes, Fletes y Acarreos", NaturalezaCuenta.Debito, true, true),
            new("5305", "Financieros", NaturalezaCuenta.Debito, false, false),
            new("530525", "Comisiones y Gastos Bancarios", NaturalezaCuenta.Debito, true, false),
            new("530535", "Perdida por Diferencia en Cambio", NaturalezaCuenta.Debito, true, false),
            new("53", "Gastos No Operacionales", NaturalezaCuenta.Debito, false, false),

            new("5195", "Diversos", NaturalezaCuenta.Debito, false, false),
            new("519520", "Actividades Deportivas", NaturalezaCuenta.Debito, true, false),
            new("519595", "Otros Gastos", NaturalezaCuenta.Debito, true, false),
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
