using FluentValidation;

namespace LAMAMedellin.Application.Features.Transacciones.Commands.RegistrarEgreso;

public sealed class RegistrarEgresoCommandValidator : AbstractValidator<RegistrarEgresoCommand>
{
    public RegistrarEgresoCommandValidator()
    {
        RuleFor(x => x.MontoCOP)
            .GreaterThan(0);

        RuleFor(x => x.CentroCostoId)
            .NotEmpty();

        RuleFor(x => x.BancoId)
            .NotEmpty();

        RuleFor(x => x.Descripcion)
            .NotEmpty()
            .MaximumLength(500);

        When(EsUsd, () =>
        {
            RuleFor(x => x.MontoMonedaOrigen)
                .NotNull()
                .GreaterThan(0);

            RuleFor(x => x.TasaCambioUsada)
                .NotNull();

            RuleFor(x => x.FechaTasaCambio)
                .NotNull();

            RuleFor(x => x.FuenteTasaCambio)
                .NotNull();
        });
    }

    private static bool EsUsd(RegistrarEgresoCommand command) =>
        string.Equals(command.MonedaOrigen, "USD", StringComparison.OrdinalIgnoreCase);
}
