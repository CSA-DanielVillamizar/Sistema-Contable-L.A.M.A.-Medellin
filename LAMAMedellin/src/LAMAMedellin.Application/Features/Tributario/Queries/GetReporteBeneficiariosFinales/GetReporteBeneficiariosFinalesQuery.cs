using MediatR;

namespace LAMAMedellin.Application.Features.Tributario.Queries.GetReporteBeneficiariosFinales;

public sealed record GetReporteBeneficiariosFinalesQuery : IRequest<IReadOnlyList<BeneficiarioFinalDto>>;
