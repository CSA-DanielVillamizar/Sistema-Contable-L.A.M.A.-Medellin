using FluentValidation;
using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Proyectos.Commands.DeleteProyectoSocial;

public sealed record DeleteProyectoSocialCommand(Guid Id) : IRequest;

public sealed class DeleteProyectoSocialCommandValidator : AbstractValidator<DeleteProyectoSocialCommand>
{
    public DeleteProyectoSocialCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class DeleteProyectoSocialCommandHandler(IProyectoSocialRepository proyectoSocialRepository)
    : IRequestHandler<DeleteProyectoSocialCommand>
{
    public async Task Handle(DeleteProyectoSocialCommand request, CancellationToken cancellationToken)
    {
        var proyecto = await proyectoSocialRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ExcepcionNegocio("Proyecto social no encontrado.");

        proyecto.MarcarComoEliminado();
        await proyectoSocialRepository.SaveChangesAsync(cancellationToken);
    }
}
