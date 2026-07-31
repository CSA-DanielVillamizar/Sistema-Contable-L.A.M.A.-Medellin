using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Commands.RegistrarEntradaInventario;

/// <summary>
/// Comando para registrar una entrada de inventario (entrada de mercancía).
/// </summary>
public sealed record RegistrarEntradaInventarioCommand(
    Guid ProductoId,
    int Cantidad,
    DateTime Fecha,
    string? Observaciones = null,
    /// <summary>
    /// Lo que costo la mercancia. Sin este dato la utilidad no se puede
    /// calcular: solo se sabria cuanto se vendio, no cuanto se gano.
    /// </summary>
    decimal? CostoUnitario = null) : IRequest<Guid>;
