using MediatR;

namespace LAMAMedellin.Application.Features.Tesoreria.Commands.ActualizarCuentaBancaria;

/// <summary>
/// Correccion de los datos identificatorios de una cuenta. El saldo no viaja
/// en el comando: se deriva de los movimientos y solo cambia registrandolos.
/// </summary>
public sealed record ActualizarCuentaBancariaCommand(
    Guid Id,
    string Nombre,
    string NumeroCuenta,
    Guid CuentaContableId) : IRequest;
