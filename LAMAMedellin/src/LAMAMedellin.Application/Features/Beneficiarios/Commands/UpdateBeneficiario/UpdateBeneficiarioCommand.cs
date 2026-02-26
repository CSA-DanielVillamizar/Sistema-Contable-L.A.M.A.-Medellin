using FluentValidation;
using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Beneficiarios.Commands.UpdateBeneficiario;

public sealed record UpdateBeneficiarioCommand(
    Guid Id,
    string NombreCompleto,
    string TipoDocumento,
    string NumeroDocumento,
    string Email,
    string Telefono,
    bool TieneConsentimientoHabeasData) : IRequest;

public sealed class UpdateBeneficiarioCommandValidator : AbstractValidator<UpdateBeneficiarioCommand>
{
    public UpdateBeneficiarioCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.NombreCompleto).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TipoDocumento).NotEmpty().MaximumLength(30);
        RuleFor(x => x.NumeroDocumento).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Telefono).NotEmpty().MaximumLength(30);
        RuleFor(x => x.TieneConsentimientoHabeasData)
            .Equal(true)
            .WithMessage("No se puede actualizar un beneficiario sin consentimiento de Habeas Data.");
    }
}

public sealed class UpdateBeneficiarioCommandHandler(IBeneficiarioRepository beneficiarioRepository)
    : IRequestHandler<UpdateBeneficiarioCommand>
{
    public async Task Handle(UpdateBeneficiarioCommand request, CancellationToken cancellationToken)
    {
        var beneficiario = await beneficiarioRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ExcepcionNegocio("Beneficiario no encontrado.");

        var beneficiarioConDocumento = await beneficiarioRepository.GetByDocumentoAsync(
            request.TipoDocumento,
            request.NumeroDocumento,
            cancellationToken);

        if (beneficiarioConDocumento is not null && beneficiarioConDocumento.Id != request.Id)
        {
            throw new ExcepcionNegocio("Ya existe un beneficiario con el mismo documento.");
        }

        beneficiario.Actualizar(
            request.NombreCompleto,
            request.TipoDocumento,
            request.NumeroDocumento,
            request.Email,
            request.Telefono,
            request.TieneConsentimientoHabeasData);

        await beneficiarioRepository.SaveChangesAsync(cancellationToken);
    }
}
