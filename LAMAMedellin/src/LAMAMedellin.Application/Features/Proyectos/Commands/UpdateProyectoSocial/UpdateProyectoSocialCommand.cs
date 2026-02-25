using FluentValidation;
using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Proyectos.Commands.UpdateProyectoSocial;

public sealed record UpdateProyectoSocialCommand(
    Guid Id,
    string Nombre,
    string Descripcion,
    DateTime FechaInicio,
    DateTime? FechaFin,
    decimal PresupuestoEstimado,
    EstadoProyectoSocial Estado) : IRequest;

public sealed class UpdateProyectoSocialCommandValidator : AbstractValidator<UpdateProyectoSocialCommand>
{
    public UpdateProyectoSocialCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.PresupuestoEstimado).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Estado).IsInEnum();
        RuleFor(x => x)
            .Must(x => !x.FechaFin.HasValue || x.FechaFin.Value.Date >= x.FechaInicio.Date)
            .WithMessage("FechaFin debe ser mayor o igual a FechaInicio.");
    }
}

public sealed class UpdateProyectoSocialCommandHandler(IProyectoSocialRepository proyectoSocialRepository)
    : IRequestHandler<UpdateProyectoSocialCommand>
{
    public async Task Handle(UpdateProyectoSocialCommand request, CancellationToken cancellationToken)
    {
        var proyecto = await proyectoSocialRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ExcepcionNegocio("Proyecto social no encontrado.");

        proyecto.Actualizar(
            request.Nombre,
            request.Descripcion,
            request.FechaInicio,
            request.FechaFin,
            request.PresupuestoEstimado,
            request.Estado);

        await proyectoSocialRepository.SaveChangesAsync(cancellationToken);
    }
}
