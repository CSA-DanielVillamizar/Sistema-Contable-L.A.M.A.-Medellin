using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Cartera.Commands.RegistrarPago;

public sealed class RegistrarPagoCuotaCommandHandler(
    ICuentaPorCobrarRepository cuentaPorCobrarRepository,
    IBancoRepository bancoRepository) : IRequestHandler<RegistrarPagoCuotaCommand, Unit>
{
    public async Task<Unit> Handle(RegistrarPagoCuotaCommand request, CancellationToken cancellationToken)
    {
        var cxc = await cuentaPorCobrarRepository.GetByIdAsync(request.CuentaPorCobrarId, cancellationToken);
        if (cxc is null)
        {
            throw new ExcepcionNegocio("La cuenta por cobrar indicada no existe.");
        }

        cxc.AplicarAbono(request.MontoCOP);

        var banco = await bancoRepository.GetDefaultAsync(cancellationToken);
        if (banco is null)
        {
            throw new ExcepcionNegocio("No hay bancos configurados para registrar el pago.");
        }

        banco.AplicarIngreso(request.MontoCOP);

        await cuentaPorCobrarRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
