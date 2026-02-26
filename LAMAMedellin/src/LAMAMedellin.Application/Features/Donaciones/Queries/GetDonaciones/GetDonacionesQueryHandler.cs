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
