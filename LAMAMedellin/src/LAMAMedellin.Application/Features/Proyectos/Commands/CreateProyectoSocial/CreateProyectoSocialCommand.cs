using FluentValidation;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Proyectos.Commands.CreateProyectoSocial;

public sealed record CreateProyectoSocialCommand(
    string Nombre,
    string Descripcion,
    DateTime FechaInicio,
    DateTime? FechaFin,
    decimal PresupuestoEstimado,
    EstadoProyectoSocial Estado) : IRequest<Guid>;

public sealed class CreateProyectoSocialCommandValidator : AbstractValidator<CreateProyectoSocialCommand>
{
    public CreateProyectoSocialCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.PresupuestoEstimado).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Estado).IsInEnum();
        RuleFor(x => x)
            .Must(x => !x.FechaFin.HasValue || x.FechaFin.Value.Date >= x.FechaInicio.Date)
            .WithMessage("FechaFin debe ser mayor o igual a FechaInicio.");
    }
}

public sealed class CreateProyectoSocialCommandHandler(IProyectoSocialRepository proyectoSocialRepository)
    : IRequestHandler<CreateProyectoSocialCommand, Guid>
{
    public async Task<Guid> Handle(CreateProyectoSocialCommand request, CancellationToken cancellationToken)
    {
        var proyecto = new ProyectoSocial(
            request.Nombre,
            request.Descripcion,
            request.FechaInicio,
            request.FechaFin,
            request.PresupuestoEstimado,
            request.Estado);

        await proyectoSocialRepository.AddAsync(proyecto, cancellationToken);
        await proyectoSocialRepository.SaveChangesAsync(cancellationToken);

        return proyecto.Id;
    }
}
