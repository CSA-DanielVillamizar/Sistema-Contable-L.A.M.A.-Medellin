namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetLibroMayor;

/// <summary>Movimiento del mayor con el saldo corrido despues de aplicarlo.</summary>
public sealed record MovimientoLibroMayorDto(
    DateTime Fecha,
    string NumeroConsecutivo,
    string DescripcionComprobante,
    string CentroCosto,
    string Referencia,
    decimal Debe,
    decimal Haber,
    decimal SaldoAcumulado);

public sealed record LibroMayorDto(
    Guid CuentaContableId,
    string CodigoCuenta,
    string DescripcionCuenta,
    string Naturaleza,
    DateOnly Desde,
    DateOnly Hasta,
    decimal SaldoAnterior,
    decimal TotalDebe,
    decimal TotalHaber,
    decimal SaldoFinal,
    IReadOnlyList<MovimientoLibroMayorDto> Movimientos);
