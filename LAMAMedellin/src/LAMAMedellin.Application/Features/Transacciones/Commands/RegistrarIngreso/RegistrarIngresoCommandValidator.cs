using FluentValidation;

namespace LAMAMedellin.Application.Features.Transacciones.Commands.RegistrarIngreso;

public sealed class RegistrarIngresoCommandValidator : AbstractValidator<RegistrarIngresoCommand>
{
    public RegistrarIngresoCommandValidator()
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

    private static bool EsUsd(RegistrarIngresoCommand command) =>
        string.Equals(command.MonedaOrigen, "USD", StringComparison.OrdinalIgnoreCase);
}
