using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Tesoreria.Queries.GetEgresos;

public sealed class GetEgresosQueryHandler(IEgresoRepository egresoRepository)
    : IRequestHandler<GetEgresosQuery, List<EgresoDto>>
{
    public async Task<List<EgresoDto>> Handle(GetEgresosQuery request, CancellationToken cancellationToken)
    {
        var egresos = await egresoRepository.GetAllWithDetallesAsync(cancellationToken);

        return egresos
            .Select(egreso => new EgresoDto(
                egreso.Id,
                egreso.Fecha,
                egreso.Monto,
                egreso.Concepto,
                egreso.TerceroId,
                egreso.CuentaContableId,
                egreso.CuentaContable?.Descripcion ?? string.Empty,
                egreso.BancoId,
                egreso.Banco?.Nombre ?? string.Empty,
                egreso.ComprobanteContableId))
            .ToList();
    }
}
