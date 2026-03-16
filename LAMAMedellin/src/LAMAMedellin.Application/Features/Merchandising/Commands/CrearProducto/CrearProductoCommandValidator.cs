using FluentValidation;

namespace LAMAMedellin.Application.Features.Merchandising.Commands.CrearProducto;

/// <summary>
/// Validador para el comando CrearProducto.
/// </summary>
public sealed class CrearProductoCommandValidator : AbstractValidator<CrearProductoCommand>
{
    public CrearProductoCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.SKU)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.PrecioVentaCOP)
            .GreaterThan(0);

        RuleFor(x => x.CuentaContableIngresoId)
            .NotEmpty();
    }
}
