using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Configuracion.MapeoContable.Commands.ActualizarMapeoContable;

public sealed class ActualizarMapeoContableCommandHandler(
    IMapeoContableRepository mapeoRepository,
    ICuentaContableRepository cuentaContableRepository)
    : IRequestHandler<ActualizarMapeoContableCommand>
{
    public async Task Handle(ActualizarMapeoContableCommand request, CancellationToken cancellationToken)
    {
        var cuenta = await cuentaContableRepository.GetByIdAsync(request.CuentaContableId, cancellationToken)
            ?? throw new ExcepcionNegocio("La cuenta contable indicada no existe.");

        // Criterio explicito de la historia: la cuenta debe admitir movimiento.
        // Mapear una operacion a una cuenta de agrupacion haria fallar todo
        // asiento que la usara, y el fallo aparecerian lejos de aqui.
        if (!cuenta.PermiteMovimiento)
        {
            throw new ExcepcionNegocio(
                $"La cuenta {cuenta.Codigo} no permite movimiento y no puede asignarse a una operacion.");
        }

        var mapeo = await mapeoRepository.GetPorOperacionAsync(request.TipoOperacion, cancellationToken);

        if (mapeo is null)
        {
            await mapeoRepository.AddAsync(
                new Domain.Entities.MapeoContable(request.TipoOperacion, request.CuentaContableId),
                cancellationToken);
        }
        else
        {
            mapeo.Reasignar(request.CuentaContableId);
        }

        // Quien cambio el mapeo y cuando lo registra BaseEntity, que es lo que
        // pide el criterio de auditoria de la historia.
        await mapeoRepository.SaveChangesAsync(cancellationToken);
    }
}
