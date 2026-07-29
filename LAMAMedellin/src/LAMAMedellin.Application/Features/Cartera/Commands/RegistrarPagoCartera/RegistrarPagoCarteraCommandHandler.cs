using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Features.Tesoreria.Commands.RegistrarIngreso;
using MediatR;

namespace LAMAMedellin.Application.Features.Cartera.Commands.RegistrarPagoCartera;

public sealed class RegistrarPagoCarteraCommandHandler(
    ICuentaPorCobrarRepository cuentaPorCobrarRepository,
    IConceptoCobroRepository conceptoCobroRepository,
    ICentroCostoRepository centroCostoRepository,
    ISender sender)
    : IRequestHandler<RegistrarPagoCarteraCommand, Unit>
{
    public async Task<Unit> Handle(RegistrarPagoCarteraCommand request, CancellationToken cancellationToken)
    {
        var cuenta = await cuentaPorCobrarRepository.GetByIdAsync(request.CuentaPorCobrarId, cancellationToken);
        if (cuenta is null)
        {
            throw new ExcepcionNegocio("La cuenta por cobrar indicada no existe.");
        }

        var concepto = await conceptoCobroRepository.GetByIdAsync(cuenta.ConceptoCobroId, cancellationToken);
        if (concepto is null)
        {
            throw new ExcepcionNegocio("El concepto de cobro asociado no existe.");
        }

        var centroCosto = (await centroCostoRepository.GetAllAsync(cancellationToken))
            .FirstOrDefault(x => !x.IsDeleted);

        if (centroCosto is null)
        {
            throw new ExcepcionNegocio("No existe un centro de costo activo para registrar el pago en tesorería.");
        }

        cuenta.AplicarPago(request.Monto);

        await sender.Send(new RegistrarIngresoCommand(
            request.Monto,
            $"Pago de Cartera: {concepto.Nombre}",
            cuenta.MiembroId,
            concepto.CuentaContableIngresoId,
            request.BancoId,
            centroCosto.Id,
            request.MedioPago), cancellationToken);

        return Unit.Value;
    }
}
