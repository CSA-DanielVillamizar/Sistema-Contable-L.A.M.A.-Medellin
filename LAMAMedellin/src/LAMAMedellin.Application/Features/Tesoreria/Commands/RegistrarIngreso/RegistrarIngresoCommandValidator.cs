using FluentValidation;

namespace LAMAMedellin.Application.Features.Tesoreria.Commands.RegistrarIngreso;

public sealed class RegistrarIngresoCommandValidator : AbstractValidator<RegistrarIngresoCommand>
{
    public RegistrarIngresoCommandValidator()
    {
        RuleFor(x => x.Fecha)
            .NotEmpty();

        RuleFor(x => x.Concepto)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Monto)
            .GreaterThan(0);

        RuleFor(x => x.CajaId)
            .NotEmpty();

        RuleFor(x => x.CuentaContableId)
            .NotEmpty();

        RuleFor(x => x.CentroCostoId)
            .NotEmpty();
    }
}
