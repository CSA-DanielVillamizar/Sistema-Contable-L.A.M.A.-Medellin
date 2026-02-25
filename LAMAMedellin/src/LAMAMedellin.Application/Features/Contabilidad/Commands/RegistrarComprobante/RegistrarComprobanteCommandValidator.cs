using FluentValidation;

namespace LAMAMedellin.Application.Features.Contabilidad.Commands.RegistrarComprobante;

public sealed class RegistrarComprobanteCommandValidator : AbstractValidator<RegistrarComprobanteCommand>
{
    public RegistrarComprobanteCommandValidator()
    {
        RuleFor(x => x.Fecha)
            .NotEmpty();

        RuleFor(x => x.Tipo)
            .IsInEnum();

        RuleFor(x => x.Descripcion)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Asientos)
            .NotNull()
            .Must(x => x.Count >= 2)
            .WithMessage("El comprobante debe tener al menos 2 líneas de asiento.");

        RuleFor(x => x)
            .Must(EstaBalanceado)
            .WithMessage("El comprobante está descuadrado (Debe != Haber)");

        RuleForEach(x => x.Asientos).ChildRules(asiento =>
        {
            asiento.RuleFor(x => x.CuentaContableId)
                .NotEmpty();

            asiento.RuleFor(x => x.CentroCostoId)
                .NotEmpty();

            asiento.RuleFor(x => x.Referencia)
                .NotEmpty()
                .MaximumLength(500);

            asiento.RuleFor(x => x.Debe)
                .GreaterThanOrEqualTo(0);

            asiento.RuleFor(x => x.Haber)
                .GreaterThanOrEqualTo(0);
        });
    }

    private static bool EstaBalanceado(RegistrarComprobanteCommand command)
    {
        var totalDebe = command.Asientos.Sum(x => x.Debe);
        var totalHaber = command.Asientos.Sum(x => x.Haber);
        return totalDebe - totalHaber == 0m;
    }
}
