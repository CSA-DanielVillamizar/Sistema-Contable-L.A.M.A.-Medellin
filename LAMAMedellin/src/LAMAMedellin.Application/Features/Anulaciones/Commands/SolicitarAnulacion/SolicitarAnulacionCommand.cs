using MediatR;

namespace LAMAMedellin.Application.Features.Anulaciones.Commands.SolicitarAnulacion;

/// <summary>Paso 1 de la historia 1-8: el Operador pide la anulacion con motivo.</summary>
public sealed record SolicitarAnulacionCommand(Guid ComprobanteId, string Motivo) : IRequest<Guid>;
