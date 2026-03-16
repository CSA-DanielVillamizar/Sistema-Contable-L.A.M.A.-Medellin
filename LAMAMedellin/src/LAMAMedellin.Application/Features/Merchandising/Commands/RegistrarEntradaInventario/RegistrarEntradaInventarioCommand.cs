using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Commands.RegistrarEntradaInventario;

/// <summary>
/// Comando para registrar una entrada de inventario (entrada de mercancía).
/// </summary>
public sealed record RegistrarEntradaInventarioCommand(
    Guid ProductoId,
    int Cantidad,
    DateTime Fecha,
    string? Observaciones = null) : IRequest<Guid>;
