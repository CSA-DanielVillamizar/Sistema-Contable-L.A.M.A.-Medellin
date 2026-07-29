using MediatR;

namespace LAMAMedellin.Application.Features.Cartera.Commands.CrearCuentaPorCobrar;

public sealed record CrearCuentaPorCobrarCommand(
    Guid MiembroId,
    Guid ConceptoCobroId,
    string Periodo,
    DateOnly FechaEmision,
    DateOnly FechaVencimiento,
    decimal ValorTotal) : IRequest<Guid>;
