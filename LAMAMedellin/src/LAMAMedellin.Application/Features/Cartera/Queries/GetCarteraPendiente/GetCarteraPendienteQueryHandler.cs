using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Cartera.Queries.GetCarteraPendiente;

public sealed class GetCarteraPendienteQueryHandler(
    ICuentaPorCobrarRepository cuentaPorCobrarRepository)
    : IRequestHandler<GetCarteraPendienteQuery, List<CarteraPendienteDto>>
{
    public async Task<List<CarteraPendienteDto>> Handle(
        GetCarteraPendienteQuery request,
        CancellationToken cancellationToken)
    {
        var cuentasPendientes = await cuentaPorCobrarRepository
            .GetByEstadoAsync(EstadoCuentaPorCobrar.Pendiente, cancellationToken);

        return cuentasPendientes
            .OrderByDescending(c => c.Periodo)
            .ThenBy(c => c.Miembro!.Nombre)
            .ThenBy(c => c.Miembro!.Apellidos)
            .Select(c => new CarteraPendienteDto(
                c.Id,
                c.MiembroId,
                $"{c.Miembro!.Nombre} {c.Miembro!.Apellidos}".Trim(),
                c.Periodo,
                c.ValorEsperadoCOP,
                c.SaldoPendienteCOP))
            .ToList();
    }
}
