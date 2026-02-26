using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Beneficiarios.Queries.GetBeneficiarios;

public sealed record GetBeneficiariosQuery : IRequest<IReadOnlyList<BeneficiarioDto>>;

public sealed record BeneficiarioDto(
    Guid Id,
    string NombreCompleto,
    string TipoDocumento,
    string NumeroDocumento,
    string Email,
    string Telefono,
    bool TieneConsentimientoHabeasData);

public sealed class GetBeneficiariosQueryHandler(IBeneficiarioRepository beneficiarioRepository)
    : IRequestHandler<GetBeneficiariosQuery, IReadOnlyList<BeneficiarioDto>>
{
    public async Task<IReadOnlyList<BeneficiarioDto>> Handle(GetBeneficiariosQuery request, CancellationToken cancellationToken)
    {
        var beneficiarios = await beneficiarioRepository.GetAllAsync(cancellationToken);

        return beneficiarios
            .Select(b => new BeneficiarioDto(
                b.Id,
                b.NombreCompleto,
                b.TipoDocumento,
                b.NumeroDocumento,
                b.Email,
                b.Telefono,
                b.TieneConsentimientoHabeasData))
            .ToList();
    }
}
