using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetPeriodosContables;

public sealed record GetPeriodosContablesQuery : IRequest<IReadOnlyList<PeriodoContableDto>>;
