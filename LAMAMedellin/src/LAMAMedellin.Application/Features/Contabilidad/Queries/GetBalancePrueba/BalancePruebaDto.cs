namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetBalancePrueba;

/// <summary>Una cuenta en el balance de prueba, con su movimiento del periodo.</summary>
public sealed record SaldoCuentaBalanceDto(
    Guid CuentaContableId,
    string CodigoCuenta,
    string DescripcionCuenta,
    string Naturaleza,
    decimal SaldoAnterior,
    decimal Debe,
    decimal Haber,
    decimal SaldoFinal);

public sealed record BalancePruebaDto(
    int Anio,
    int Mes,
    decimal TotalDebe,
    decimal TotalHaber,
    /// <summary>
    /// La suma de debitos debe igualar la de creditos. Si sale false hay una
    /// inconsistencia en el libro que debe investigarse antes de cerrar.
    /// </summary>
    bool EstaCuadrado,
    IReadOnlyList<SaldoCuentaBalanceDto> Cuentas);
