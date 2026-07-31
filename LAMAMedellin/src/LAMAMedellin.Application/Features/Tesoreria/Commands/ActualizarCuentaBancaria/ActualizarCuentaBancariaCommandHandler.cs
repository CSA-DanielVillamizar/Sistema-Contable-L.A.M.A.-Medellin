using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Common;
using MediatR;

namespace LAMAMedellin.Application.Features.Tesoreria.Commands.ActualizarCuentaBancaria;

public sealed class ActualizarCuentaBancariaCommandHandler(
    IBancoRepository bancoRepository,
    ICuentaContableRepository cuentaContableRepository)
    : IRequestHandler<ActualizarCuentaBancariaCommand>
{
    public async Task Handle(ActualizarCuentaBancariaCommand request, CancellationToken cancellationToken)
    {
        var banco = await bancoRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ReglaNegocioException($"La cuenta bancaria con Id {request.Id} no existe.");

        var cuentaContable = await cuentaContableRepository.GetByIdAsync(request.CuentaContableId, cancellationToken)
            ?? throw new ReglaNegocioException(
                $"La cuenta contable con Id {request.CuentaContableId} no existe.");

        if (!cuentaContable.Codigo.StartsWith("11", StringComparison.Ordinal))
        {
            throw new ReglaNegocioException(
                "La cuenta contable de una cuenta bancaria debe pertenecer al disponible (codigo 11xx).");
        }

        if (await bancoRepository.ExisteNumeroCuentaAsync(request.NumeroCuenta, request.Id, cancellationToken))
        {
            throw new ReglaNegocioException(
                $"Ya existe otra cuenta bancaria con el numero {request.NumeroCuenta}.");
        }

        banco.ActualizarDatos(request.Nombre, request.NumeroCuenta, request.CuentaContableId);

        await bancoRepository.SaveChangesAsync(cancellationToken);
    }
}
