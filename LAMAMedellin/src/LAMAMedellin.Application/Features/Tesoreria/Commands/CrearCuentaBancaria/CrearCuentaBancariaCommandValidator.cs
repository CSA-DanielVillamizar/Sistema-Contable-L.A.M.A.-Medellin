using FluentValidation;

namespace LAMAMedellin.Application.Features.Tesoreria.Commands.CrearCuentaBancaria;

public sealed class CrearCuentaBancariaCommandValidator : AbstractValidator<CrearCuentaBancariaCommand>
{
    public CrearCuentaBancariaCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("Nombre es obligatorio.")
            .MaximumLength(150).WithMessage("Nombre no puede superar 150 caracteres.");

        RuleFor(x => x.NumeroCuenta)
            .NotEmpty().WithMessage("NumeroCuenta es obligatorio.")
            .MaximumLength(50).WithMessage("NumeroCuenta no puede superar 50 caracteres.");

        RuleFor(x => x.CuentaContableId)
            .NotEmpty().WithMessage("CuentaContableId es obligatorio.");
    }
}
