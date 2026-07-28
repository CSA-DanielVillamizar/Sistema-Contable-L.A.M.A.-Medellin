namespace LAMAMedellin.Application.Features.Tesoreria.Queries.GetCuentasBancarias;

public sealed record CuentaBancariaDto(
    Guid Id,
    string Nombre,
    string NumeroCuenta,
    decimal SaldoActual,
    bool EsActivo);
