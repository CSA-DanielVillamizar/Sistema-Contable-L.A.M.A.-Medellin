using MediatR;

namespace LAMAMedellin.Application.Features.Configuracion.MapeoContable.Queries.GetMapeosContables;

public sealed record GetMapeosContablesQuery : IRequest<IReadOnlyList<MapeoContableDto>>;

/// <summary>
/// Una operacion con la cuenta que tiene asignada. CuentaContableId viene nulo
/// cuando la operacion aun no se ha configurado: la pantalla lo muestra como
/// pendiente en vez de esconderlo.
/// </summary>
public sealed record MapeoContableDto(
    int TipoOperacion,
    string NombreOperacion,
    Guid? CuentaContableId,
    string? CodigoCuenta,
    string? DescripcionCuenta);
