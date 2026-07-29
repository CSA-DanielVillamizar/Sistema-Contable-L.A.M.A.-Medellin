using MediatR;

namespace LAMAMedellin.Application.Features.Tesoreria.Queries.GetCuentasBancarias;

/// <summary>Cuentas bancarias disponibles. Por defecto solo las activas.</summary>
public sealed record GetCuentasBancariasQuery(bool IncluirInactivas = false)
    : IRequest<IReadOnlyList<CuentaBancariaDto>>;
