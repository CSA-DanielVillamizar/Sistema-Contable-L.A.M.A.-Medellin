using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetCuentasContables;

public sealed record GetCuentasContablesQuery : IRequest<IReadOnlyList<CuentaContableDto>>;

public sealed record CuentaContableDto(
    Guid Id,
    string Codigo,
    string Descripcion,
    string Naturaleza,
    bool PermiteMovimiento,
    bool ExigeTercero);
