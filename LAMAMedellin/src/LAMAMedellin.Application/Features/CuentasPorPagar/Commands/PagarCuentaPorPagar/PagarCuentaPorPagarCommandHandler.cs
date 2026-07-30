using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Features.Tesoreria.Commands.RegistrarEgreso;
using MediatR;

namespace LAMAMedellin.Application.Features.CuentasPorPagar.Commands.PagarCuentaPorPagar;

public sealed class PagarCuentaPorPagarCommandHandler(
    ICuentaPorPagarRepository cuentaPorPagarRepository,
    ISender sender)
    : IRequestHandler<PagarCuentaPorPagarCommand>
{
    public async Task Handle(PagarCuentaPorPagarCommand request, CancellationToken cancellationToken)
    {
        var cuenta = await cuentaPorPagarRepository.GetByIdAsync(request.CuentaPorPagarId, cancellationToken)
            ?? throw new ExcepcionNegocio("La cuenta por pagar indicada no existe.");

        // Se aplica primero contra la obligacion: si el monto excede el saldo,
        // la regla salta aqui y no llega a moverse dinero del banco.
        cuenta.AplicarPago(request.Monto);

        await sender.Send(new RegistrarEgresoCommand(
            request.Monto,
            $"Pago factura {cuenta.NumeroFactura} - {cuenta.NombreProveedor}",
            null,
            cuenta.CuentaContableGastoId,
            request.BancoId,
            cuenta.CentroCostoId,
            request.MedioPago), cancellationToken);

        await cuentaPorPagarRepository.SaveChangesAsync(cancellationToken);
    }
}
