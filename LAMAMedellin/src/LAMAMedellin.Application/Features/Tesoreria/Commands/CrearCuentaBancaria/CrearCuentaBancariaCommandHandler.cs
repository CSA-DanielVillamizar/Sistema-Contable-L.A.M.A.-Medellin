using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Entities;
using MediatR;

namespace LAMAMedellin.Application.Features.Tesoreria.Commands.CrearCuentaBancaria;

public sealed class CrearCuentaBancariaCommandHandler(
    IBancoRepository bancoRepository,
    ICuentaContableRepository cuentaContableRepository)
    : IRequestHandler<CrearCuentaBancariaCommand, Guid>
{
    public async Task<Guid> Handle(CrearCuentaBancariaCommand request, CancellationToken cancellationToken)
    {
        var cuentaContable = await cuentaContableRepository.GetByIdAsync(request.CuentaContableId, cancellationToken)
            ?? throw new ReglaNegocioException(
                $"La cuenta contable con Id {request.CuentaContableId} no existe.");

        // Sin esta comprobacion la contrapartida de los movimientos caeria en
        // una cuenta que no representa disponible, y el balance dejaria de
        // cuadrar por rubro aunque la partida doble siguiera equilibrada.
        if (!cuentaContable.Codigo.StartsWith("11", StringComparison.Ordinal))
        {
            throw new ReglaNegocioException(
                "La cuenta contable de una cuenta bancaria debe pertenecer al disponible (codigo 11xx).");
        }

        if (await bancoRepository.ExisteNumeroCuentaAsync(request.NumeroCuenta, null, cancellationToken))
        {
            throw new ReglaNegocioException(
                $"Ya existe una cuenta bancaria con el numero {request.NumeroCuenta}.");
        }

        var banco = new Banco(
            nombre: request.Nombre,
            numeroCuenta: request.NumeroCuenta,
            saldoActual: 0m,
            cuentaContableId: request.CuentaContableId);

        await bancoRepository.AddAsync(banco, cancellationToken);
        await bancoRepository.SaveChangesAsync(cancellationToken);

        return banco.Id;
    }
}
