using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Commands.CrearProducto;

/// <summary>
/// Comando para crear un nuevo producto en el sistema.
/// </summary>
public sealed record CrearProductoCommand(
    string Nombre,
    string SKU,
    decimal PrecioVentaCOP,
    Guid CuentaContableIngresoId) : IRequest<Guid>;
