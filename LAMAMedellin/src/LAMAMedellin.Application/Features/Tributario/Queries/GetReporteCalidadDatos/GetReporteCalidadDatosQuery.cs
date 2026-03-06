using MediatR;

namespace LAMAMedellin.Application.Features.Tributario.Queries.GetReporteCalidadDatos;

public sealed record GetReporteCalidadDatosQuery : IRequest<IReadOnlyList<InconsistenciaTributariaDto>>;
