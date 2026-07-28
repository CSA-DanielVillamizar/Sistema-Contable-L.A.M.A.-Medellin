using FluentValidation;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetLibroMayor;

public sealed class GetLibroMayorQueryValidator : AbstractValidator<GetLibroMayorQuery>
{
    public GetLibroMayorQueryValidator()
    {
        RuleFor(x => x.CuentaContableId).NotEmpty();

        RuleFor(x => x.Hasta)
            .GreaterThanOrEqualTo(x => x.Desde)
            .WithMessage("La fecha final no puede ser anterior a la inicial.");
    }
}
