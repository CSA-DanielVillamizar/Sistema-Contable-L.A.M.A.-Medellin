using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using MediatR;

namespace LAMAMedellin.Application.Features.Cartera.Commands.CrearCuentaPorCobrar;

public sealed class CrearCuentaPorCobrarCommandHandler(
    IMiembroRepository miembroRepository,
    IConceptoCobroRepository conceptoCobroRepository,
    ICuentaPorCobrarRepository cuentaPorCobrarRepository)
    : IRequestHandler<CrearCuentaPorCobrarCommand, Guid>
{
    public async Task<Guid> Handle(CrearCuentaPorCobrarCommand request, CancellationToken cancellationToken)
    {
        // Validar que el miembro existe
        var miembro = await miembroRepository.GetByIdAsync(request.MiembroId, cancellationToken);
        if (miembro is null)
        {
            throw new ExcepcionNegocio($"El miembro con Id {request.MiembroId} no existe.");
        }

        // Validar que el concepto de cobro existe
        var conceptoCobro = await conceptoCobroRepository.GetByIdAsync(
            request.ConceptoCobroId,
            cancellationToken);

        if (conceptoCobro is null)
        {
            throw new ExcepcionNegocio(
                $"El concepto de cobro con Id {request.ConceptoCobroId} no existe.");
        }

        // Crear la cuenta por cobrar
        var cuentaPorCobrar = new CuentaPorCobrar(
            request.MiembroId,
            request.ConceptoCobroId,
            request.Periodo,
            request.FechaEmision,
            request.FechaVencimiento,
            request.ValorTotal);

        await cuentaPorCobrarRepository.AddAsync(cuentaPorCobrar, cancellationToken);
        await cuentaPorCobrarRepository.SaveChangesAsync(cancellationToken);

        return cuentaPorCobrar.Id;
    }
}
