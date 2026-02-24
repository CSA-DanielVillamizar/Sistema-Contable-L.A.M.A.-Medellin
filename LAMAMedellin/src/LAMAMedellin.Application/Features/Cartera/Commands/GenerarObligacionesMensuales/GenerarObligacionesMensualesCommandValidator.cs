using FluentValidation;

namespace LAMAMedellin.Application.Features.Cartera.Commands.GenerarObligacionesMensuales;

public sealed class GenerarObligacionesMensualesCommandValidator : AbstractValidator<GenerarObligacionesMensualesCommand>
{
    public GenerarObligacionesMensualesCommandValidator()
    {
        RuleFor(x => x.Periodo)
            .NotEmpty()
            .Matches(@"^\d{4}-(0[1-9]|1[0-2])$");
    }
}
