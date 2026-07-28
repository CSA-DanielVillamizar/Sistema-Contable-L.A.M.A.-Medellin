using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Common.Interfaces.Services;
using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Commands.CerrarPeriodo;

/// <summary>
/// Cierre ejecutado por el Contador (historia 1-5).
///
/// Antes de cerrar se verifica que no queden comprobantes en Borrador: un
/// borrador dentro del periodo es un movimiento a medio registrar, y cerrar
/// encima lo dejaria en un limbo del que ya no se puede sacar sin un ajuste.
/// </summary>
public sealed class CerrarPeriodoCommandHandler(
    IPeriodoContableRepository periodoRepository,
    IUsuarioActual usuarioActual)
    : IRequestHandler<CerrarPeriodoCommand, Unit>
{
    public async Task<Unit> Handle(CerrarPeriodoCommand request, CancellationToken cancellationToken)
    {
        var periodo = await periodoRepository.GetPorAnioYMesAsync(request.Anio, request.Mes, cancellationToken);

        if (periodo is null)
        {
            throw new ExcepcionNegocio(
                "El periodo no existe. Debe ser validado por tesoreria antes de cerrarse.");
        }

        var borradores = await periodoRepository.ContarComprobantesEnBorradorAsync(
            request.Anio,
            request.Mes,
            cancellationToken);

        if (borradores > 0)
        {
            throw new ExcepcionNegocio(
                $"El periodo tiene {borradores} comprobante(s) en borrador. " +
                "Asientelos o anulelos antes de cerrar.");
        }

        periodo.Cerrar(usuarioActual.Identificador);

        await periodoRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
