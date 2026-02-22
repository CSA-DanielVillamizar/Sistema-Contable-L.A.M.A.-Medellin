using MediatR;

namespace LAMAMedellin.Application.Features.Dashboard.Queries.GetResumenCartera;

public sealed record GetResumenCarteraQuery : IRequest<ResumenCarteraDto>;
