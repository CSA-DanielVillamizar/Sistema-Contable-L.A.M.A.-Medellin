using FluentValidation;

namespace LAMAMedellin.Application.Features.Donaciones.Commands.RegistrarDonacion;

public sealed class RegistrarDonacionCommandValidator : AbstractValidator<RegistrarDonacionCommand>
{
    public RegistrarDonacionCommandValidator()
    {
        RuleFor(x => x.DonanteId)
            .NotEmpty();

        RuleFor(x => x.MontoCOP)
            .GreaterThan(0);

        RuleFor(x => x.BancoId)
            .NotEmpty();

        RuleFor(x => x.CentroCostoId)
            .NotEmpty();

        RuleFor(x => x.MedioPago)
            .IsInEnum();

        RuleFor(x => x.FormaDonacion)
            .IsInEnum();

        RuleFor(x => x.MedioPagoODescripcion)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Descripcion)
            .MaximumLength(500);
    }
}
