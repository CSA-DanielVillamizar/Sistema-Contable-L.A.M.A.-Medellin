using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Queries.GetDonaciones;

public sealed record GetDonacionesQuery : IRequest<IReadOnlyList<DonacionDto>>;
