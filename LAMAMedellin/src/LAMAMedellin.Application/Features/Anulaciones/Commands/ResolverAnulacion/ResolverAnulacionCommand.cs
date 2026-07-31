using MediatR;

namespace LAMAMedellin.Application.Features.Anulaciones.Commands.ResolverAnulacion;

/// <summary>
/// Paso 2 de la historia 1-8: el Tesorero aprueba o rechaza. Si aprueba, el
/// comprobante queda anulado en el mismo acto.
/// </summary>
public sealed record ResolverAnulacionCommand(Guid SolicitudId, bool Aprobar, string? Motivo) : IRequest;
