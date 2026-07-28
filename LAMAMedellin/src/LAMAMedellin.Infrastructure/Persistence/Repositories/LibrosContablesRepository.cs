using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Features.Contabilidad.Queries.GetBalancePrueba;
using LAMAMedellin.Application.Features.Contabilidad.Queries.GetLibroDiario;
using LAMAMedellin.Application.Features.Contabilidad.Queries.GetLibroMayor;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class LibrosContablesRepository(LamaDbContext dbContext) : ILibrosContablesRepository
{
    public async Task<IReadOnlyList<MovimientoLibroDiarioDto>> GetLibroDiarioAsync(
        DateOnly desde,
        DateOnly hasta,
        Guid? centroCostoId,
        CancellationToken cancellationToken = default)
    {
        var consulta = ConstruirConsultaAsientos(desde, hasta, centroCostoId);

        return await consulta
            .OrderBy(x => x.Comprobante!.Fecha)
            .ThenBy(x => x.Comprobante!.NumeroConsecutivo)
            .Select(x => new MovimientoLibroDiarioDto(
                x.Comprobante!.Fecha,
                x.Comprobante.NumeroConsecutivo,
                x.Comprobante.TipoComprobante.ToString(),
                x.Comprobante.Descripcion,
                x.CuentaContable!.Codigo,
                x.CuentaContable.Descripcion,
                x.CentroCosto!.Nombre,
                x.TerceroId,
                x.Referencia,
                x.Debe,
                x.Haber))
            .ToListAsync(cancellationToken);
    }

    public Task<CuentaContable?> GetCuentaAsync(
        Guid cuentaContableId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.CuentasContables
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == cuentaContableId, cancellationToken);
    }

    public async Task<IReadOnlyList<MovimientoLibroMayorDto>> GetMovimientosMayorAsync(
        Guid cuentaContableId,
        DateOnly desde,
        DateOnly hasta,
        Guid? centroCostoId,
        CancellationToken cancellationToken = default)
    {
        var consulta = ConstruirConsultaAsientos(desde, hasta, centroCostoId)
            .Where(x => x.CuentaContableId == cuentaContableId);

        // El saldo acumulado se calcula en memoria: depende del orden de los
        // movimientos y de la naturaleza de la cuenta, y expresarlo en SQL no
        // aporta nada frente al volumen que maneja la fundacion.
        return await consulta
            .OrderBy(x => x.Comprobante!.Fecha)
            .ThenBy(x => x.Comprobante!.NumeroConsecutivo)
            .Select(x => new MovimientoLibroMayorDto(
                x.Comprobante!.Fecha,
                x.Comprobante.NumeroConsecutivo,
                x.Comprobante.Descripcion,
                x.CentroCosto!.Nombre,
                x.Referencia,
                x.Debe,
                x.Haber,
                0m))
            .ToListAsync(cancellationToken);
    }

    public async Task<(decimal Debe, decimal Haber)> GetAcumuladoAnteriorAsync(
        Guid cuentaContableId,
        DateOnly desde,
        Guid? centroCostoId,
        CancellationToken cancellationToken = default)
    {
        var limite = desde.ToDateTime(TimeOnly.MinValue);

        var consulta = dbContext.AsientosContables
            .AsNoTracking()
            .Where(x => x.CuentaContableId == cuentaContableId)
            .Where(x => x.Comprobante!.EstadoComprobante == EstadoComprobante.Asentado)
            .Where(x => x.Comprobante!.Fecha < limite);

        if (centroCostoId.HasValue)
        {
            consulta = consulta.Where(x => x.CentroCostoId == centroCostoId.Value);
        }

        var totales = await consulta
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Debe = g.Sum(x => x.Debe),
                Haber = g.Sum(x => x.Haber),
            })
            .FirstOrDefaultAsync(cancellationToken);

        return (totales?.Debe ?? 0m, totales?.Haber ?? 0m);
    }

    public async Task<IReadOnlyList<SaldoCuentaBalanceDto>> GetBalancePruebaAsync(
        int anio,
        int mes,
        Guid? centroCostoId,
        CancellationToken cancellationToken = default)
    {
        var inicio = new DateTime(anio, mes, 1, 0, 0, 0, DateTimeKind.Utc);
        var finExclusivo = inicio.AddMonths(1);

        var asientos = dbContext.AsientosContables
            .AsNoTracking()
            .Where(x => x.Comprobante!.EstadoComprobante == EstadoComprobante.Asentado);

        if (centroCostoId.HasValue)
        {
            asientos = asientos.Where(x => x.CentroCostoId == centroCostoId.Value);
        }

        // Se resuelve con dos agregaciones simples en vez de una sola con sumas
        // condicionales dentro del GroupBy: esa forma depende de que el
        // proveedor sepa traducirla, y aqui no hay margen para que una consulta
        // contable falle en ejecucion. Ambas son GroupBy + Sum planos.
        var anterior = await AgruparPorCuentaAsync(
            asientos.Where(x => x.Comprobante!.Fecha < inicio),
            cancellationToken);

        var periodo = await AgruparPorCuentaAsync(
            asientos.Where(x => x.Comprobante!.Fecha >= inicio && x.Comprobante.Fecha < finExclusivo),
            cancellationToken);

        var porCuenta = anterior
            .Concat(periodo)
            .GroupBy(x => x.CuentaContableId)
            .Select(g => g.First())
            .ToDictionary(x => x.CuentaContableId);

        return porCuenta.Values
            .Select(cuenta =>
            {
                var previo = anterior.FirstOrDefault(x => x.CuentaContableId == cuenta.CuentaContableId);
                var actual = periodo.FirstOrDefault(x => x.CuentaContableId == cuenta.CuentaContableId);

                var debeAnterior = previo?.Debe ?? 0m;
                var haberAnterior = previo?.Haber ?? 0m;
                var debePeriodo = actual?.Debe ?? 0m;
                var haberPeriodo = actual?.Haber ?? 0m;

                return new SaldoCuentaBalanceDto(
                    cuenta.CuentaContableId,
                    cuenta.Codigo,
                    cuenta.Descripcion,
                    cuenta.Naturaleza.ToString(),
                    CalcularSaldo(cuenta.Naturaleza, debeAnterior, haberAnterior),
                    debePeriodo,
                    haberPeriodo,
                    CalcularSaldo(
                        cuenta.Naturaleza,
                        debeAnterior + debePeriodo,
                        haberAnterior + haberPeriodo));
            })
            .Where(x => x.SaldoAnterior != 0m || x.Debe != 0m || x.Haber != 0m)
            .OrderBy(x => x.CodigoCuenta)
            .ToList();
    }

    /// <summary>
    /// El signo del saldo depende de la naturaleza: una cuenta de naturaleza
    /// debito (activo, gasto) crece con el debe, y una de naturaleza credito
    /// (pasivo, patrimonio, ingreso) crece con el haber. Restar siempre en el
    /// mismo sentido mostraria los ingresos en negativo.
    /// </summary>
    public static decimal CalcularSaldo(NaturalezaCuenta naturaleza, decimal debe, decimal haber) =>
        naturaleza == NaturalezaCuenta.Debito ? debe - haber : haber - debe;

    private sealed record TotalesCuenta(
        Guid CuentaContableId,
        string Codigo,
        string Descripcion,
        NaturalezaCuenta Naturaleza,
        decimal Debe,
        decimal Haber);

    private static async Task<List<TotalesCuenta>> AgruparPorCuentaAsync(
        IQueryable<AsientoContable> asientos,
        CancellationToken cancellationToken)
    {
        return await asientos
            .GroupBy(x => new
            {
                x.CuentaContableId,
                x.CuentaContable!.Codigo,
                x.CuentaContable.Descripcion,
                x.CuentaContable.Naturaleza,
            })
            .Select(g => new TotalesCuenta(
                g.Key.CuentaContableId,
                g.Key.Codigo,
                g.Key.Descripcion,
                g.Key.Naturaleza,
                g.Sum(x => x.Debe),
                g.Sum(x => x.Haber)))
            .ToListAsync(cancellationToken);
    }

    private IQueryable<AsientoContable> ConstruirConsultaAsientos(
        DateOnly desde,
        DateOnly hasta,
        Guid? centroCostoId)
    {
        var inicio = desde.ToDateTime(TimeOnly.MinValue);
        // Fin exclusivo: incluye cualquier hora del ultimo dia del rango.
        var finExclusivo = hasta.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var consulta = dbContext.AsientosContables
            .AsNoTracking()
            .Include(x => x.Comprobante)
            .Include(x => x.CuentaContable)
            .Include(x => x.CentroCosto)
            .Where(x => x.Comprobante!.EstadoComprobante == EstadoComprobante.Asentado)
            .Where(x => x.Comprobante!.Fecha >= inicio && x.Comprobante.Fecha < finExclusivo);

        if (centroCostoId.HasValue)
        {
            consulta = consulta.Where(x => x.CentroCostoId == centroCostoId.Value);
        }

        return consulta;
    }
}
