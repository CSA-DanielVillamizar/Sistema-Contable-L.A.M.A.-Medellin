using FluentValidation;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetLibroDiario;

public sealed class GetLibroDiarioQueryValidator : AbstractValidator<GetLibroDiarioQuery>
{
    public GetLibroDiarioQueryValidator()
    {
        RuleFor(x => x.Hasta)
            .GreaterThanOrEqualTo(x => x.Desde)
            .WithMessage("La fecha final no puede ser anterior a la inicial.");
    }
}
