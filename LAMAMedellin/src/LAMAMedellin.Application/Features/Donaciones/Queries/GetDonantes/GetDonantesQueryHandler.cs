using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Queries.GetDonantes;

public sealed class GetDonantesQueryHandler(IDonanteRepository donanteRepository)
    : IRequestHandler<GetDonantesQuery, IReadOnlyList<DonanteDto>>
{
    public async Task<IReadOnlyList<DonanteDto>> Handle(GetDonantesQuery request, CancellationToken cancellationToken)
    {
        var donantes = await donanteRepository.GetAllAsync(cancellationToken);

        return donantes
            .OrderBy(x => x.NombreORazonSocial)
            .Select(x => new DonanteDto(
                x.Id,
                x.NombreORazonSocial,
                x.TipoDocumento.ToString(),
                x.NumeroDocumento,
                x.Email,
                x.TipoPersona.ToString()))
            .ToList();
    }
}
