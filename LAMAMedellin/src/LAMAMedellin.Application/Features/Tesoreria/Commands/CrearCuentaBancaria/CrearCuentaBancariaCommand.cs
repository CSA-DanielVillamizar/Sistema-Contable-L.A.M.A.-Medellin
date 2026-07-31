using MediatR;

namespace LAMAMedellin.Application.Features.Tesoreria.Commands.CrearCuentaBancaria;

/// <summary>
/// Alta de una cuenta bancaria. No recibe saldo inicial a proposito: el saldo
/// es el resultado de los movimientos, y sembrarlo a mano lo desligaria del
/// libro desde el primer dia. Si la cuenta ya trae saldo, se registra como un
/// ingreso con su comprobante.
/// </summary>
public sealed record CrearCuentaBancariaCommand(
    string Nombre,
    string NumeroCuenta,
    Guid CuentaContableId) : IRequest<Guid>;
