using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Commands.CrearProducto;

/// <summary>
/// Comando para crear un nuevo producto en el sistema.
/// </summary>
/// <summary>
/// El formulario ya capturaba stock inicial y cantidad minima, pero el comando
/// no los recibia: se creaban productos en cero y el usuario no se enteraba.
/// Los nombres se alinean con los que envia el frontend y con la entidad.
/// </summary>
public sealed record CrearProductoCommand(
    string Nombre,
    string CodigoSKU,
    decimal PrecioVenta,
    int CantidadEnStock,
    int CantidadMinima,
    Guid CuentaContableIngresoId) : IRequest<Guid>;
