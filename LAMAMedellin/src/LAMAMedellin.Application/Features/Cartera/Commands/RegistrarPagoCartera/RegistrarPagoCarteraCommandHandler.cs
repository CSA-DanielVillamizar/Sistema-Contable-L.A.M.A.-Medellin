using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Cartera.Commands.RegistrarPagoCartera;

public sealed class RegistrarPagoCarteraCommandHandler(
    ICuentaPorCobrarRepository cuentaPorCobrarRepository,
    IBancoRepository bancoRepository,
    ICentroCostoRepository centroCostoRepository,
    ITransaccionRepository transaccionRepository)
    : IRequestHandler<RegistrarPagoCarteraCommand, Unit>
{
    public async Task<Unit> Handle(RegistrarPagoCarteraCommand request, CancellationToken cancellationToken)
    {
        var cartera = await cuentaPorCobrarRepository.GetByIdAsync(request.CarteraId, cancellationToken);
        if (cartera is null)
        {
            throw new ExcepcionNegocio("La cuenta por cobrar indicada no existe.");
        }

        var banco = await bancoRepository.GetByIdAsync(request.BancoId, cancellationToken);
        if (banco is null)
        {
            throw new ExcepcionNegocio("El banco indicado no existe.");
        }

        var centroCosto = await centroCostoRepository.GetByIdAsync(request.CentroCostoId, cancellationToken);
        if (centroCosto is null)
        {
            throw new ExcepcionNegocio("El centro de costo indicado no existe.");
        }

        cartera.AplicarAbono(request.MontoPagadoCOP);
        banco.AplicarIngreso(request.MontoPagadoCOP);

        var descripcion = string.IsNullOrWhiteSpace(request.Descripcion)
            ? $"Pago cartera periodo {cartera.Periodo}"
            : request.Descripcion.Trim();

        var transaccion = new Transaccion(
            request.MontoPagadoCOP,
            DateTime.UtcNow,
            TipoTransaccion.Ingreso,
            MedioPago.Transferencia,
            request.CentroCostoId,
            request.BancoId,
            descripcion);

        await transaccionRepository.AddAsync(transaccion, cancellationToken);
        await transaccionRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
