using MediatR;

namespace LAMAMedellin.Application.Features.Transacciones.Queries.GetTransacciones;

public sealed record GetTransaccionesQuery : IRequest<List<TransaccionDto>>;
