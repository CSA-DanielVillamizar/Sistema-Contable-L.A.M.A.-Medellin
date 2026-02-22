using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Enums;
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
            .GetByEstadoAsync(EstadoCuentaPorCobrar.Pendiente, cancellationToken);

        var totalPendiente = cuentasPendientes.Sum(cuenta => cuenta.SaldoPendienteCOP);

        return new ResumenCarteraDto(totalPendiente);
    }
}
