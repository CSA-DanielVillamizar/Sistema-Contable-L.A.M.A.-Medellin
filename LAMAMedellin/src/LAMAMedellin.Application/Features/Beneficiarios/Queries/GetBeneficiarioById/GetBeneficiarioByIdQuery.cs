using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Beneficiarios.Queries.GetBeneficiarioById;

public sealed record GetBeneficiarioByIdQuery(Guid Id) : IRequest<BeneficiarioDto?>;

public sealed record BeneficiarioDto(
    Guid Id,
    string NombreCompleto,
    string TipoDocumento,
    string NumeroDocumento,
    string Email,
    string Telefono,
    bool TieneConsentimientoHabeasData);

public sealed class GetBeneficiarioByIdQueryHandler(IBeneficiarioRepository beneficiarioRepository)
    : IRequestHandler<GetBeneficiarioByIdQuery, BeneficiarioDto?>
{
    public async Task<BeneficiarioDto?> Handle(GetBeneficiarioByIdQuery request, CancellationToken cancellationToken)
    {
        var beneficiario = await beneficiarioRepository.GetByIdAsync(request.Id, cancellationToken);
        if (beneficiario is null)
        {
            return null;
        }

        return new BeneficiarioDto(
            beneficiario.Id,
            beneficiario.NombreCompleto,
            beneficiario.TipoDocumento,
            beneficiario.NumeroDocumento,
            beneficiario.Email,
            beneficiario.Telefono,
            beneficiario.TieneConsentimientoHabeasData);
    }
}
