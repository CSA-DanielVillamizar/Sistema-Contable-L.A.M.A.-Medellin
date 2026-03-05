using FluentValidation;

namespace LAMAMedellin.Application.Features.Tributario.Queries.GetReporteExogena;

public sealed class GetReporteExogenaQueryValidator : AbstractValidator<GetReporteExogenaQuery>
{
    public GetReporteExogenaQueryValidator()
    {
        RuleFor(x => x.Anio)
            .InclusiveBetween(2000, 2100)
            .WithMessage("El año debe estar entre 2000 y 2100.");

        RuleFor(x => x.Mes)
            .InclusiveBetween(1, 12)
            .When(x => x.Mes.HasValue)
            .WithMessage("El mes debe estar entre 1 y 12.");
    }
}
