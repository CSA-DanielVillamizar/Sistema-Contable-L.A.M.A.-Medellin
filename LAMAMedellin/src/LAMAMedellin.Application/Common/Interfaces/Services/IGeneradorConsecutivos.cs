using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Application.Common.Interfaces.Services;

public interface IGeneradorConsecutivos
{
    /// <summary>
    /// Reserva y devuelve el siguiente numero consecutivo del tipo indicado,
    /// de forma atomica. Si la operacion que lo pidio se revierte dentro de una
    /// transaccion, el consecutivo se revierte con ella y no queda hueco.
    /// </summary>
    Task<string> SiguienteAsync(TipoComprobante tipoComprobante, CancellationToken cancellationToken = default);
}
