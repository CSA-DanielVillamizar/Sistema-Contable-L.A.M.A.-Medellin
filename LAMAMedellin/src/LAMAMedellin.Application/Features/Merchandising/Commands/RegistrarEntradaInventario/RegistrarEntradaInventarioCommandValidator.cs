using FluentValidation;

namespace LAMAMedellin.Application.Features.Merchandising.Commands.RegistrarEntradaInventario;

/// <summary>
/// Validador para el comando RegistrarEntradaInventario.
/// </summary>
public sealed class RegistrarEntradaInventarioCommandValidator : AbstractValidator<RegistrarEntradaInventarioCommand>
{
    public RegistrarEntradaInventarioCommandValidator()
    {
        RuleFor(x => x.ProductoId)
            .NotEmpty();

        RuleFor(x => x.Cantidad)
            .GreaterThan(0)
            .WithMessage("La cantidad debe ser mayor a cero para registrar una entrada.");

        RuleFor(x => x.Fecha)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("La fecha no puede ser en el futuro.");

        RuleFor(x => x.Observaciones)
            .MaximumLength(500);
    }
}
