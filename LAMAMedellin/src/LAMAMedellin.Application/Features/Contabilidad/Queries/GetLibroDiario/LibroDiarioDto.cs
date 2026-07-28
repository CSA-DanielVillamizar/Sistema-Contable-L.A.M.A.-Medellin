namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetLibroDiario;

/// <summary>
/// Una linea del libro diario: un asiento con su comprobante, en orden
/// cronologico. Es el registro cronologico de todos los movimientos.
/// </summary>
public sealed record MovimientoLibroDiarioDto(
    DateTime Fecha,
    string NumeroConsecutivo,
    string TipoComprobante,
    string DescripcionComprobante,
    string CodigoCuenta,
    string DescripcionCuenta,
    string CentroCosto,
    Guid? TerceroId,
    string Referencia,
    decimal Debe,
    decimal Haber);

public sealed record LibroDiarioDto(
    DateOnly Desde,
    DateOnly Hasta,
    decimal TotalDebe,
    decimal TotalHaber,
    bool EstaCuadrado,
    IReadOnlyList<MovimientoLibroDiarioDto> Movimientos);
