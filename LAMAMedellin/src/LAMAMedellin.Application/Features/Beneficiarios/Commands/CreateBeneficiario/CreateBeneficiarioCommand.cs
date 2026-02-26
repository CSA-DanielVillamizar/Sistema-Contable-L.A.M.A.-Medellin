using FluentValidation;
using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using MediatR;

namespace LAMAMedellin.Application.Features.Beneficiarios.Commands.CreateBeneficiario;

public sealed record CreateBeneficiarioCommand(
    string NombreCompleto,
    string TipoDocumento,
    string NumeroDocumento,
    string Email,
    string Telefono,
    bool TieneConsentimientoHabeasData) : IRequest<Guid>;

public sealed class CreateBeneficiarioCommandValidator : AbstractValidator<CreateBeneficiarioCommand>
{
    public CreateBeneficiarioCommandValidator()
    {
        RuleFor(x => x.NombreCompleto).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TipoDocumento).NotEmpty().MaximumLength(30);
        RuleFor(x => x.NumeroDocumento).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Telefono).NotEmpty().MaximumLength(30);
        RuleFor(x => x.TieneConsentimientoHabeasData)
            .Equal(true)
            .WithMessage("No se puede registrar un beneficiario sin consentimiento de Habeas Data.");
    }
}

public sealed class CreateBeneficiarioCommandHandler(IBeneficiarioRepository beneficiarioRepository)
    : IRequestHandler<CreateBeneficiarioCommand, Guid>
{
    public async Task<Guid> Handle(CreateBeneficiarioCommand request, CancellationToken cancellationToken)
    {
        var beneficiarioExistente = await beneficiarioRepository.GetByDocumentoAsync(
            request.TipoDocumento,
            request.NumeroDocumento,
            cancellationToken);

        if (beneficiarioExistente is not null)
        {
            throw new ExcepcionNegocio("Ya existe un beneficiario con el mismo documento.");
        }

        var beneficiario = new Beneficiario(
            request.NombreCompleto,
            request.TipoDocumento,
            request.NumeroDocumento,
            request.Email,
            request.Telefono,
            request.TieneConsentimientoHabeasData);

        await beneficiarioRepository.AddAsync(beneficiario, cancellationToken);
        await beneficiarioRepository.SaveChangesAsync(cancellationToken);

        return beneficiario.Id;
    }
}
