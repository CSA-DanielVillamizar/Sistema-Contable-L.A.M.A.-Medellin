using FluentValidation;

namespace LAMAMedellin.Application.Features.Cartera.Commands.GenerarRenovacionAnual;

public sealed class GenerarRenovacionAnualCommandValidator : AbstractValidator<GenerarRenovacionAnualCommand>
{
    public GenerarRenovacionAnualCommandValidator()
    {
        RuleFor(x => x.Anio)
            .InclusiveBetween(2020, 2100)
            .WithMessage("Anio debe ser un valor razonable.");

        RuleFor(x => x.TasaCambioUsada)
            .GreaterThan(0)
            .WithMessage("TasaCambioUsada debe ser mayor a cero.");
    }
}
