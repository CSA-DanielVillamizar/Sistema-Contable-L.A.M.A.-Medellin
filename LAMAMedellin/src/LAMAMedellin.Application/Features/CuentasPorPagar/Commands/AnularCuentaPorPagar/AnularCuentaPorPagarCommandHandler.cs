using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.CuentasPorPagar.Commands.AnularCuentaPorPagar;

public sealed class AnularCuentaPorPagarCommandHandler(ICuentaPorPagarRepository cuentaPorPagarRepository)
    : IRequestHandler<AnularCuentaPorPagarCommand>
{
    public async Task Handle(AnularCuentaPorPagarCommand request, CancellationToken cancellationToken)
    {
        var cuenta = await cuentaPorPagarRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ExcepcionNegocio("La cuenta por pagar indicada no existe.");

        cuenta.Anular();

        await cuentaPorPagarRepository.SaveChangesAsync(cancellationToken);
    }
}
