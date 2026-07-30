using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Queries.GetDonaciones;

public sealed class GetDonacionesQueryHandler(IDonacionRepository donacionRepository)
    : IRequestHandler<GetDonacionesQuery, IReadOnlyList<DonacionDto>>
{
    public async Task<IReadOnlyList<DonacionDto>> Handle(GetDonacionesQuery request, CancellationToken cancellationToken)
    {
        var donaciones = await donacionRepository.GetAllWithDetallesAsync(cancellationToken);

        return donaciones
            .Where(x => request.Desde is null || DateOnly.FromDateTime(x.Fecha) >= request.Desde)
            .Where(x => request.Hasta is null || DateOnly.FromDateTime(x.Fecha) <= request.Hasta)
            .Where(x => request.DonanteId is null || x.DonanteId == request.DonanteId)
            .Where(x => request.CentroCostoId is null || x.CentroCostoId == request.CentroCostoId)
            .Where(x => request.CertificadoEmitido is null || x.CertificadoEmitido == request.CertificadoEmitido)
            .OrderByDescending(x => x.Fecha)
            .Select(x => new DonacionDto(
                x.Id,
                x.DonanteId,
                x.Donante?.NombreORazonSocial ?? string.Empty,
                x.MontoCOP,
                x.Fecha,
                x.BancoId,
                x.CentroCostoId,
                x.CertificadoEmitido,
                x.CodigoVerificacion,
                x.FormaDonacion,
                x.MedioPagoODescripcion))
            .ToList();
    }
}
