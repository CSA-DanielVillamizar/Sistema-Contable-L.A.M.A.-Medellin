using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Tributario.Queries.GetReporteBeneficiariosFinales;

public sealed class GetReporteBeneficiariosFinalesQueryHandler(ITributarioRepository tributarioRepository)
    : IRequestHandler<GetReporteBeneficiariosFinalesQuery, IReadOnlyList<BeneficiarioFinalDto>>
{
    public async Task<IReadOnlyList<BeneficiarioFinalDto>> Handle(GetReporteBeneficiariosFinalesQuery request, CancellationToken cancellationToken)
    {
        return await tributarioRepository.GetReporteBeneficiariosFinalesAsync(cancellationToken);
    }
}
