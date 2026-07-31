using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Configuracion.MapeoContable.Commands.ActualizarMapeoContable;

/// <summary>
/// Asigna la cuenta contable de una operacion (historia 1-2). Si la operacion
/// aun no tenia mapeo, lo crea.
/// </summary>
public sealed record ActualizarMapeoContableCommand(
    TipoOperacionContable TipoOperacion,
    Guid CuentaContableId) : IRequest;
