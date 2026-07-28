using MediatR;

namespace LAMAMedellin.Application.Features.Eventos.Commands.EstablecerCuotaLogistica;

/// <summary>
/// Establece o actualiza la cuota logística en COP de un evento.
/// Solo aplica mientras el evento no haya sido finalizado o cancelado.
/// Enviar <c>null</c> en <see cref="CuotaLogisticaCOP"/> elimina la cuota del evento.
/// </summary>
public sealed record EstablecerCuotaLogisticaCommand(
    Guid EventoId,
    decimal? CuotaLogisticaCOP) : IRequest<Unit>;
