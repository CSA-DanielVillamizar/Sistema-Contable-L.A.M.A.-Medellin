using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Anulaciones.Queries.GetSolicitudesAnulacion;

public sealed class GetSolicitudesAnulacionQueryHandler(
    ISolicitudAnulacionRepository solicitudRepository,
    IComprobanteRepository comprobanteRepository)
    : IRequestHandler<GetSolicitudesAnulacionQuery, IReadOnlyList<SolicitudAnulacionDto>>
{
    public async Task<IReadOnlyList<SolicitudAnulacionDto>> Handle(
        GetSolicitudesAnulacionQuery request,
        CancellationToken cancellationToken)
    {
        var solicitudes = await solicitudRepository.GetAllAsync(cancellationToken);
        var resultado = new List<SolicitudAnulacionDto>(solicitudes.Count);

        foreach (var s in solicitudes)
        {
            var comprobante = await comprobanteRepository.GetByIdWithAsientosAsync(s.ComprobanteId, cancellationToken);

            resultado.Add(new SolicitudAnulacionDto(
                s.Id,
                s.ComprobanteId,
                comprobante?.NumeroConsecutivo ?? string.Empty,
                comprobante?.Fecha ?? default,
                comprobante?.Descripcion ?? string.Empty,
                s.MotivoSolicitud,
                (int)s.Estado,
                // Quien solicito lo aporta la pista de auditoria de BaseEntity.
                s.CreatedBy,
                s.CreatedAt,
                s.ResueltaPor,
                s.FechaResolucion,
                s.MotivoResolucion));
        }

        // Lo pendiente primero: es sobre lo que hay que actuar.
        return resultado
            .OrderBy(x => x.Estado)
            .ThenByDescending(x => x.FechaSolicitud)
            .ToList();
    }
}
