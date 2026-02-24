using FluentValidation;

namespace LAMAMedellin.Application.Features.Cartera.Commands.RegistrarPagoCartera;

public sealed class RegistrarPagoCarteraCommandValidator : AbstractValidator<RegistrarPagoCarteraCommand>
{
    public RegistrarPagoCarteraCommandValidator()
    {
        RuleFor(x => x.CarteraId)
            .NotEmpty();

        RuleFor(x => x.MontoPagadoCOP)
            .GreaterThan(0);

        RuleFor(x => x.BancoId)
            .NotEmpty();

        RuleFor(x => x.CentroCostoId)
            .NotEmpty();

        RuleFor(x => x.Descripcion)
            .MaximumLength(500);
    }
}
