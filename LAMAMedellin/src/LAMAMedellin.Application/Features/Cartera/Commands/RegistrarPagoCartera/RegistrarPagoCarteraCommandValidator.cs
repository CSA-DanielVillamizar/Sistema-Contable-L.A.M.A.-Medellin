using FluentValidation;

namespace LAMAMedellin.Application.Features.Cartera.Commands.RegistrarPagoCartera;

public sealed class RegistrarPagoCarteraCommandValidator : AbstractValidator<RegistrarPagoCarteraCommand>
{
    public RegistrarPagoCarteraCommandValidator()
    {
        RuleFor(x => x.CuentaPorCobrarId)
            .NotEmpty();

        RuleFor(x => x.Monto)
            .GreaterThan(0);

        RuleFor(x => x.BancoId)
            .NotEmpty();
    }
}
