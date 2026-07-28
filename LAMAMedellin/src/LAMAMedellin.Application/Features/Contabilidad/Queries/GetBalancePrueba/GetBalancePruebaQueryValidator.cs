using FluentValidation;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetBalancePrueba;

public sealed class GetBalancePruebaQueryValidator : AbstractValidator<GetBalancePruebaQuery>
{
    public GetBalancePruebaQueryValidator()
    {
        RuleFor(x => x.Anio).InclusiveBetween(2000, 2999);
        RuleFor(x => x.Mes).InclusiveBetween(1, 12);
    }
}
