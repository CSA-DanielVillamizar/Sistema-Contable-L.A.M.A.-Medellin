using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Anulaciones.Commands.SolicitarAnulacion;

public sealed class SolicitarAnulacionCommandHandler(
    ISolicitudAnulacionRepository solicitudRepository,
    IComprobanteRepository comprobanteRepository)
    : IRequestHandler<SolicitarAnulacionCommand, Guid>
{
    public async Task<Guid> Handle(SolicitarAnulacionCommand request, CancellationToken cancellationToken)
    {
        var comprobante = await comprobanteRepository.GetByIdWithAsientosAsync(request.ComprobanteId, cancellationToken)
            ?? throw new ExcepcionNegocio("El comprobante indicado no existe.");

        if (comprobante.EstadoComprobante == EstadoComprobante.Anulado)
        {
            throw new ExcepcionNegocio("El comprobante ya esta anulado.");
        }

        // La regla del mes se comprueba ya al solicitar, no solo al aprobar: no
        // tiene sentido dejar en cola una solicitud que nunca podra aprobarse.
        var ahora = DateTime.UtcNow;
        if (comprobante.Fecha.Year != ahora.Year || comprobante.Fecha.Month != ahora.Month)
        {
            throw new ExcepcionNegocio(
                "Solo se puede anular un comprobante dentro de su mismo mes contable. "
                + "Para un mes anterior corresponde registrar un ajuste contable.");
        }

        if (await solicitudRepository.ExistePendienteAsync(request.ComprobanteId, cancellationToken))
        {
            throw new ExcepcionNegocio("Ya existe una solicitud de anulacion pendiente para este comprobante.");
        }

        var solicitud = new SolicitudAnulacion(request.ComprobanteId, request.Motivo);

        await solicitudRepository.AddAsync(solicitud, cancellationToken);
        await solicitudRepository.SaveChangesAsync(cancellationToken);

        return solicitud.Id;
    }
}
