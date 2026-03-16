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

        RuleFor(x => x.CajaId)
            .NotEmpty();

        RuleFor(x => x.CentroCostoId)
            .NotEmpty();

        RuleFor(x => x.Fecha)
            .NotEmpty();

        RuleFor(x => x.Observaciones)
            .MaximumLength(500);
    }
}
