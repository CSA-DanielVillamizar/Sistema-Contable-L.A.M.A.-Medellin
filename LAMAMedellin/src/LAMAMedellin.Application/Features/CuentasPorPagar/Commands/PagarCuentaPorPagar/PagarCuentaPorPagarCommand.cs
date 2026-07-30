using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.CuentasPorPagar.Commands.PagarCuentaPorPagar;

/// <summary>
/// Paga una obligacion registrada y la cruza (historia 1-14). Genera el egreso
/// bancario, de modo que el saldo del banco y el pasivo bajan a la vez.
/// </summary>
public sealed record PagarCuentaPorPagarCommand(
    Guid CuentaPorPagarId,
    decimal Monto,
    Guid BancoId,
    MedioPago MedioPago,
    /// <summary>Solo para obligaciones en USD: cuantos dolares se liquidaron.</summary>
    decimal? MontoUSD = null,
    /// <summary>Tasa a la que se liquido. Frente a la reconocida produce la diferencia.</summary>
    decimal? TasaCambioLiquidacion = null) : IRequest;
