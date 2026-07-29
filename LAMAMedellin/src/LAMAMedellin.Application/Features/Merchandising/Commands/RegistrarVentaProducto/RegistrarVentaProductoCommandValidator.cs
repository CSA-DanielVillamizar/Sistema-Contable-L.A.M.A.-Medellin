using FluentValidation;

namespace LAMAMedellin.Application.Features.Merchandising.Commands.RegistrarVentaProducto;

public sealed class RegistrarVentaProductoCommandValidator : AbstractValidator<RegistrarVentaProductoCommand>
{
    public RegistrarVentaProductoCommandValidator()
    {
        RuleFor(x => x.ProductoId)
            .NotEmpty();

        RuleFor(x => x.Cantidad)
            .GreaterThan(0);

        RuleFor(x => x.BancoId)
            .NotEmpty();

        RuleFor(x => x.Concepto)
            .NotEmpty()
            .MaximumLength(200);
    }
}
