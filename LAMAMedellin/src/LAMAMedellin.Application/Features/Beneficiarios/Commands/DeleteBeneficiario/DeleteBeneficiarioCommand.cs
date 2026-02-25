using FluentValidation;
using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Beneficiarios.Commands.DeleteBeneficiario;

public sealed record DeleteBeneficiarioCommand(Guid Id) : IRequest;

public sealed class DeleteBeneficiarioCommandValidator : AbstractValidator<DeleteBeneficiarioCommand>
{
    public DeleteBeneficiarioCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class DeleteBeneficiarioCommandHandler(IBeneficiarioRepository beneficiarioRepository)
    : IRequestHandler<DeleteBeneficiarioCommand>
{
    public async Task Handle(DeleteBeneficiarioCommand request, CancellationToken cancellationToken)
    {
        var beneficiario = await beneficiarioRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ExcepcionNegocio("Beneficiario no encontrado.");

        beneficiario.MarcarComoEliminado();
        await beneficiarioRepository.SaveChangesAsync(cancellationToken);
    }
}
