using FluentValidation;

namespace LAMAMedellin.Application.Features.CuentasPorPagar.Commands.PagarCuentaPorPagar;

public sealed class PagarCuentaPorPagarCommandValidator : AbstractValidator<PagarCuentaPorPagarCommand>
{
    public PagarCuentaPorPagarCommandValidator()
    {
        RuleFor(x => x.CuentaPorPagarId)
            .NotEmpty().WithMessage("CuentaPorPagarId es obligatorio.");

        RuleFor(x => x.Monto)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a cero.");

        RuleFor(x => x.BancoId)
            .NotEmpty().WithMessage("BancoId es obligatorio.");

        RuleFor(x => x.MedioPago)
            .IsInEnum().WithMessage("MedioPago debe ser un valor valido.");
    }
}
