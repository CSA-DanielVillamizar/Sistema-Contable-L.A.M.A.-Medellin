using MediatR;

namespace LAMAMedellin.Application.Features.Tributario.Queries.GetReporteExogena;

public sealed record GetReporteExogenaQuery(int Anio, int? Mes) : IRequest<IReadOnlyList<ReporteExogenaDto>>;
