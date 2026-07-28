using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Dashboard.Queries.GetResumenCartera;

public sealed class GetResumenCarteraQueryHandler(
    ICuentaPorCobrarRepository cuentaPorCobrarRepository)
    : IRequestHandler<GetResumenCarteraQuery, ResumenCarteraDto>
{
    public async Task<ResumenCarteraDto> Handle(
        GetResumenCarteraQuery request,
        CancellationToken cancellationToken)
    {
        var cuentasPendientes = await cuentaPorCobrarRepository
            .GetPendientesAsync(cancellationToken);

        var totalPendiente = cuentasPendientes.Sum(cuenta => cuenta.SaldoPendiente);

        return new ResumenCarteraDto(totalPendiente);
    }
}
