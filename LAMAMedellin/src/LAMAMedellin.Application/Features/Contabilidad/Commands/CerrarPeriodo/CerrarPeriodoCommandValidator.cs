using FluentValidation;

namespace LAMAMedellin.Application.Features.Contabilidad.Commands.CerrarPeriodo;

public sealed class CerrarPeriodoCommandValidator : AbstractValidator<CerrarPeriodoCommand>
{
    public CerrarPeriodoCommandValidator()
    {
        RuleFor(x => x.Anio).InclusiveBetween(2000, 2999);
        RuleFor(x => x.Mes).InclusiveBetween(1, 12);
    }
}
