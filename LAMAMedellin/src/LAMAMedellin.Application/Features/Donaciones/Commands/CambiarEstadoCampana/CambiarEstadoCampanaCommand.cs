using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Commands.CambiarEstadoCampana;

/// <summary>
/// Cierra o reabre una campana. Se prefiere sobre el borrado porque lo
/// recaudado bajo ella debe seguir existiendo.
/// </summary>
public sealed record CambiarEstadoCampanaCommand(Guid Id, bool Activa) : IRequest;
