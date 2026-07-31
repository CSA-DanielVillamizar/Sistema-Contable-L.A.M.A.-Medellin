using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Common.Interfaces.Services;
using MediatR;

namespace LAMAMedellin.Application.Features.Anulaciones.Commands.ResolverAnulacion;

public sealed class ResolverAnulacionCommandHandler(
    ISolicitudAnulacionRepository solicitudRepository,
    IComprobanteRepository comprobanteRepository,
    IUsuarioActual usuarioActual)
    : IRequestHandler<ResolverAnulacionCommand>
{
    public async Task Handle(ResolverAnulacionCommand request, CancellationToken cancellationToken)
    {
        var solicitud = await solicitudRepository.GetByIdAsync(request.SolicitudId, cancellationToken)
            ?? throw new ExcepcionNegocio("La solicitud de anulacion indicada no existe.");

        var quienResuelve = usuarioActual.Identificador
            ?? throw new ExcepcionNegocio("No se pudo determinar quien resuelve la solicitud.");

        if (!request.Aprobar)
        {
            solicitud.Rechazar(quienResuelve, request.Motivo ?? string.Empty);
            await solicitudRepository.SaveChangesAsync(cancellationToken);
            return;
        }

        var comprobante = await comprobanteRepository.GetByIdWithAsientosAsync(solicitud.ComprobanteId, cancellationToken)
            ?? throw new ExcepcionNegocio("El comprobante asociado ya no existe.");

        // Se anula primero: si el comprobante quedo fuera de su mes mientras la
        // solicitud esperaba, la regla salta aqui y la solicitud sigue
        // pendiente en vez de quedar aprobada sobre algo que no se anulo.
        comprobante.Anular(DateTime.UtcNow);

        solicitud.Aprobar(quienResuelve, request.Motivo);

        await solicitudRepository.SaveChangesAsync(cancellationToken);
    }
}
