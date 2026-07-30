using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.CuentasPorPagar.Queries.GetCuentasPorPagar;

public sealed class GetCuentasPorPagarQueryHandler(
    ICuentaPorPagarRepository cuentaPorPagarRepository,
    ICuentaContableRepository cuentaContableRepository,
    ICentroCostoRepository centroCostoRepository)
    : IRequestHandler<GetCuentasPorPagarQuery, IReadOnlyList<CuentaPorPagarDto>>
{
    public async Task<IReadOnlyList<CuentaPorPagarDto>> Handle(
        GetCuentasPorPagarQuery request,
        CancellationToken cancellationToken)
    {
        var cuentas = await cuentaPorPagarRepository.GetAllAsync(cancellationToken);
        var cuentasContables = await cuentaContableRepository.GetAllAsync(cancellationToken);
        var centrosCosto = await centroCostoRepository.GetAllAsync(cancellationToken);

        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

        return cuentas
            .Where(c => request.IncluirAnuladas || c.Estado != EstadoCuentaPorPagar.Anulada)
            .OrderBy(c => c.FechaVencimiento)
            .Select(c => new CuentaPorPagarDto(
                c.Id,
                c.NombreProveedor,
                c.NitProveedor,
                c.NumeroFactura,
                c.Concepto,
                cuentasContables.FirstOrDefault(x => x.Id == c.CuentaContableGastoId)?.Codigo ?? string.Empty,
                centrosCosto.FirstOrDefault(x => x.Id == c.CentroCostoId)?.Nombre ?? string.Empty,
                c.FechaEmision,
                c.FechaVencimiento,
                c.ValorTotal,
                c.SaldoPendiente,
                (int)c.Estado,
                // Vencida es la que ya paso su fecha y aun debe algo. Es lo
                // primero que el tesorero necesita ver.
                c.FechaVencimiento < hoy && c.SaldoPendiente > 0))
            .ToList();
    }
}
