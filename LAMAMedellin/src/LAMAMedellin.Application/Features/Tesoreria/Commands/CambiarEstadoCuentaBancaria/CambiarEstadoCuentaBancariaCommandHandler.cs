using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Common;
using MediatR;

namespace LAMAMedellin.Application.Features.Tesoreria.Commands.CambiarEstadoCuentaBancaria;

public sealed class CambiarEstadoCuentaBancariaCommandHandler(IBancoRepository bancoRepository)
    : IRequestHandler<CambiarEstadoCuentaBancariaCommand>
{
    public async Task Handle(CambiarEstadoCuentaBancariaCommand request, CancellationToken cancellationToken)
    {
        var banco = await bancoRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ReglaNegocioException($"La cuenta bancaria con Id {request.Id} no existe.");

        if (request.EsActivo)
        {
            banco.Activar();
        }
        else
        {
            // Dar de baja una cuenta con dinero sacaria ese saldo del total del
            // tablero sin que ningun movimiento lo explique. Primero hay que
            // trasladarlo, que es un hecho contable y debe quedar registrado.
            if (banco.SaldoActual != 0m)
            {
                throw new ReglaNegocioException(
                    "No se puede desactivar una cuenta con saldo. Traslade el saldo a otra cuenta primero.");
            }

            banco.Desactivar();
        }

        await bancoRepository.SaveChangesAsync(cancellationToken);
    }
}
