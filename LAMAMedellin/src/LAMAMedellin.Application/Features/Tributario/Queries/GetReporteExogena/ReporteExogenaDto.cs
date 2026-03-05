namespace LAMAMedellin.Application.Features.Tributario.Queries.GetReporteExogena;

public sealed record ReporteExogenaDto(
    string TerceroId,
    string NombreTercero,
    string CuentaContableCodigo,
    string CuentaContableNombre,
    decimal TotalDebito,
    decimal TotalCredito,
    decimal SaldoMovimiento);
