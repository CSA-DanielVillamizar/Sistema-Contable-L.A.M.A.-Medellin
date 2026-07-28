using FluentValidation;

namespace LAMAMedellin.Application.Features.Configuracion.CuotasAsamblea.Commands.ActualizarRenovacionMembresia;

public sealed class ActualizarRenovacionMembresiaCommandValidator : AbstractValidator<ActualizarRenovacionMembresiaCommand>
{
    public ActualizarRenovacionMembresiaCommandValidator()
    {
        RuleFor(x => x.Anio)
            .InclusiveBetween(2000, 2100)
            .WithMessage("El año debe estar entre 2000 y 2100.");

        RuleFor(x => x.RenovacionMembresiaUSD)
            .GreaterThan(0)
            .When(x => x.RenovacionMembresiaUSD.HasValue)
            .WithMessage("RenovacionMembresiaUSD debe ser mayor a cero.");
    }
}
