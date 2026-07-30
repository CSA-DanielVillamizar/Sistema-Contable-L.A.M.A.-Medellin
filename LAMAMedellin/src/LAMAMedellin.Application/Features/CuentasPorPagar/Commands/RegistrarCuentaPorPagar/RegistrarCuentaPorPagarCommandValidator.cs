using FluentValidation;

namespace LAMAMedellin.Application.Features.CuentasPorPagar.Commands.RegistrarCuentaPorPagar;

public sealed class RegistrarCuentaPorPagarCommandValidator : AbstractValidator<RegistrarCuentaPorPagarCommand>
{
    public RegistrarCuentaPorPagarCommandValidator()
    {
        RuleFor(x => x.NombreProveedor)
            .NotEmpty().WithMessage("NombreProveedor es obligatorio.")
            .MaximumLength(200).WithMessage("NombreProveedor no puede superar 200 caracteres.");

        RuleFor(x => x.NitProveedor)
            .NotEmpty().WithMessage("NitProveedor es obligatorio.")
            .MaximumLength(30).WithMessage("NitProveedor no puede superar 30 caracteres.");

        RuleFor(x => x.NumeroFactura)
            .NotEmpty().WithMessage("NumeroFactura es obligatorio.")
            .MaximumLength(50).WithMessage("NumeroFactura no puede superar 50 caracteres.");

        RuleFor(x => x.Concepto)
            .NotEmpty().WithMessage("Concepto es obligatorio.")
            .MaximumLength(500).WithMessage("Concepto no puede superar 500 caracteres.");

        RuleFor(x => x.CuentaContableGastoId)
            .NotEmpty().WithMessage("CuentaContableGastoId es obligatorio.");

        RuleFor(x => x.CentroCostoId)
            .NotEmpty().WithMessage("CentroCostoId es obligatorio.");

        RuleFor(x => x.ValorTotal)
            .GreaterThan(0).WithMessage("ValorTotal debe ser mayor a cero.");

        RuleFor(x => x.FechaVencimiento)
            .GreaterThanOrEqualTo(x => x.FechaEmision)
            .WithMessage("FechaVencimiento no puede ser anterior a FechaEmision.");
    }
}
