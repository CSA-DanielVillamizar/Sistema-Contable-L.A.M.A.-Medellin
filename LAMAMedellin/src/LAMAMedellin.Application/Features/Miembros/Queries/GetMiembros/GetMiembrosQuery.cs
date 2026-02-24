using MediatR;

namespace LAMAMedellin.Application.Features.Miembros.Queries.GetMiembros;

public sealed record GetMiembrosQuery : IRequest<IReadOnlyList<MiembroDto>>;
