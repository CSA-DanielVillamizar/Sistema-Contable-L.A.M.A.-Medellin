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

        RuleFor(x => x.CodigoSKU)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.PrecioVenta)
            .GreaterThan(0);

        RuleFor(x => x.CuentaContableIngresoId)
            .NotEmpty();

        RuleFor(x => x.CantidadEnStock)
            .GreaterThanOrEqualTo(0).WithMessage("CantidadEnStock no puede ser negativa.");

        RuleFor(x => x.CantidadMinima)
            .GreaterThanOrEqualTo(0).WithMessage("CantidadMinima no puede ser negativa.");
    }
}
