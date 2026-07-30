namespace LAMAMedellin.Application.Features.CuentasPorPagar.Queries.GetCuentasPorPagar;

public sealed record CuentaPorPagarDto(
    Guid Id,
    string NombreProveedor,
    string NitProveedor,
    string NumeroFactura,
    string Concepto,
    string CodigoCuentaGasto,
    string NombreCentroCosto,
    DateOnly FechaEmision,
    DateOnly FechaVencimiento,
    decimal ValorTotal,
    decimal SaldoPendiente,
    int Estado,
    bool EstaVencida);
