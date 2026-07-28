using LAMAMedellin.Application.Common.Interfaces.Repositories;
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
            .GetPendientesAsync(cancellationToken);

        return cuentasPendientes
            .OrderByDescending(c => c.FechaVencimiento)
            .ThenBy(c => c.Miembro!.Nombres)
            .ThenBy(c => c.Miembro!.Apellidos)
            .Select(c => new CarteraPendienteDto(
                c.Id,
                c.MiembroId,
                $"{c.Miembro!.Nombres} {c.Miembro!.Apellidos}".Trim(),
                c.FechaEmision,
                c.ValorTotal,
                c.SaldoPendiente))
            .ToList();
    }
}
