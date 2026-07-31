using MediatR;

namespace LAMAMedellin.Application.Features.Anulaciones.Queries.GetSolicitudesAnulacion;

public sealed record GetSolicitudesAnulacionQuery : IRequest<IReadOnlyList<SolicitudAnulacionDto>>;

public sealed record SolicitudAnulacionDto(
    Guid Id,
    Guid ComprobanteId,
    string NumeroConsecutivo,
    DateTime FechaComprobante,
    string DescripcionComprobante,
    string MotivoSolicitud,
    int Estado,
    string? SolicitadaPor,
    DateTime? FechaSolicitud,
    string? ResueltaPor,
    DateTime? FechaResolucion,
    string? MotivoResolucion);
