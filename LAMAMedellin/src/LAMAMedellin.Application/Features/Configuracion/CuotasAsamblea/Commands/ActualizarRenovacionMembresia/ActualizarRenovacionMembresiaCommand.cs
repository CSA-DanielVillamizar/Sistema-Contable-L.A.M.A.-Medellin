using MediatR;

namespace LAMAMedellin.Application.Features.Configuracion.CuotasAsamblea.Commands.ActualizarRenovacionMembresia;

/// <summary>
/// Actualiza el valor en USD de la cuota de renovación de membresía internacional
/// para el año indicado. El valor es configurable por año (BRD §4.4).
/// Enviar <c>null</c> en <see cref="RenovacionMembresiaUSD"/> elimina el cobro para ese año.
/// </summary>
public sealed record ActualizarRenovacionMembresiaCommand(
    int Anio,
    decimal? RenovacionMembresiaUSD) : IRequest<Unit>;
