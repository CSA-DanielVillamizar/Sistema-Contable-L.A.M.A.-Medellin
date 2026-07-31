using MediatR;

namespace LAMAMedellin.Application.Features.CuentasPorPagar.Queries.GetCuentasPorPagar;

/// <summary>Cuentas por pagar. Sin filtro devuelve todas menos las anuladas.</summary>
public sealed record GetCuentasPorPagarQuery(bool IncluirAnuladas = false)
    : IRequest<IReadOnlyList<CuentaPorPagarDto>>;
