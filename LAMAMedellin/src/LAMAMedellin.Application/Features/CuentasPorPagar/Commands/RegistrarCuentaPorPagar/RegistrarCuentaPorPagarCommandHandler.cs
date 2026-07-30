using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using MediatR;

namespace LAMAMedellin.Application.Features.CuentasPorPagar.Commands.RegistrarCuentaPorPagar;

public sealed class RegistrarCuentaPorPagarCommandHandler(
    ICuentaPorPagarRepository cuentaPorPagarRepository,
    ICuentaContableRepository cuentaContableRepository,
    ICentroCostoRepository centroCostoRepository)
    : IRequestHandler<RegistrarCuentaPorPagarCommand, Guid>
{
    public async Task<Guid> Handle(RegistrarCuentaPorPagarCommand request, CancellationToken cancellationToken)
    {
        var cuentaGasto = await cuentaContableRepository.GetByIdAsync(request.CuentaContableGastoId, cancellationToken)
            ?? throw new ExcepcionNegocio("La cuenta contable indicada no existe.");

        if (!cuentaGasto.PermiteMovimiento)
        {
            throw new ExcepcionNegocio("La cuenta contable indicada no permite movimiento.");
        }

        // Una factura de proveedor reconoce un gasto o un costo. Imputarla a un
        // ingreso o a un activo dejaria el estado de resultados sin sentido.
        if (!cuentaGasto.Codigo.StartsWith("5", StringComparison.Ordinal)
            && !cuentaGasto.Codigo.StartsWith("6", StringComparison.Ordinal))
        {
            throw new ExcepcionNegocio(
                "La cuenta de una factura de proveedor debe ser de gasto (5xxx) o de costo (6xxx).");
        }

        _ = await centroCostoRepository.GetByIdAsync(request.CentroCostoId, cancellationToken)
            ?? throw new ExcepcionNegocio("El centro de costo indicado no existe.");

        if (await cuentaPorPagarRepository.ExisteFacturaAsync(
                request.NitProveedor, request.NumeroFactura, cancellationToken))
        {
            throw new ExcepcionNegocio(
                $"La factura {request.NumeroFactura} del proveedor {request.NitProveedor} ya esta registrada.");
        }

        var cuenta = new CuentaPorPagar(
            request.NombreProveedor,
            request.NitProveedor,
            request.NumeroFactura,
            request.Concepto,
            request.CuentaContableGastoId,
            request.CentroCostoId,
            request.FechaEmision,
            request.FechaVencimiento,
            request.ValorTotal,
            request.ValorUSD,
            request.TasaCambioReconocida);

        await cuentaPorPagarRepository.AddAsync(cuenta, cancellationToken);
        await cuentaPorPagarRepository.SaveChangesAsync(cancellationToken);

        return cuenta.Id;
    }
}
