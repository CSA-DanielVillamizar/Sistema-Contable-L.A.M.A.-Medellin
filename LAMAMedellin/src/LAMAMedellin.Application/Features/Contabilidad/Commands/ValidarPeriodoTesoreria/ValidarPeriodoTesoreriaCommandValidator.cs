using FluentValidation;

namespace LAMAMedellin.Application.Features.Contabilidad.Commands.ValidarPeriodoTesoreria;

public sealed class ValidarPeriodoTesoreriaCommandValidator
    : AbstractValidator<ValidarPeriodoTesoreriaCommand>
{
    public ValidarPeriodoTesoreriaCommandValidator()
    {
        RuleFor(x => x.Anio).InclusiveBetween(2000, 2999);
        RuleFor(x => x.Mes).InclusiveBetween(1, 12);
    }
}
