using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Queries.GetDonantes;

public sealed record GetDonantesQuery : IRequest<IReadOnlyList<DonanteDto>>;
