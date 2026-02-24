using FluentValidation;

namespace LAMAMedellin.Application.Features.Cartera.Commands.GenerarCarteraMensual;

public sealed class GenerarCarteraMensualCommandValidator : AbstractValidator<GenerarCarteraMensualCommand>
{
    public GenerarCarteraMensualCommandValidator()
    {
        RuleFor(x => x.Periodo)
            .NotEmpty()
            .Matches("^\\d{4}-(0[1-9]|1[0-2])$")
            .WithMessage("Periodo debe tener formato YYYY-MM.");
    }
}
