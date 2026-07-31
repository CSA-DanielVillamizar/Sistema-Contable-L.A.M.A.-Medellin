using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Recibos.Queries.VerificarRecibo;

public sealed class VerificarReciboQueryHandler(IComprobanteRepository comprobanteRepository)
    : IRequestHandler<VerificarReciboQuery, ReciboVerificadoDto?>
{
    public async Task<ReciboVerificadoDto?> Handle(
        VerificarReciboQuery request,
        CancellationToken cancellationToken)
    {
        var comprobante = await comprobanteRepository.GetPorConsecutivoAsync(
            request.NumeroConsecutivo,
            cancellationToken);

        // Devolver null y no una excepcion: un consecutivo inventado es un caso
        // normal en un endpoint publico, no un error del sistema.
        if (comprobante is null)
        {
            return null;
        }

        var valor = comprobante.AsientosContables.Sum(a => a.Debe);

        return new ReciboVerificadoDto(
            comprobante.NumeroConsecutivo,
            comprobante.Fecha,
            valor,
            comprobante.EstadoComprobante.ToString(),
            // Un comprobante anulado sigue existiendo, pero el recibo que lo
            // respalda ya no vale. Quien verifica necesita saberlo.
            comprobante.EstadoComprobante != EstadoComprobante.Anulado);
    }
}
