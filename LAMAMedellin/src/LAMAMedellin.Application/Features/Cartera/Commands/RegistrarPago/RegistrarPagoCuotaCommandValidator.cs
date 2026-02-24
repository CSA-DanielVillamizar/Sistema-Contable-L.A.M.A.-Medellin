using FluentValidation;

namespace LAMAMedellin.Application.Features.Cartera.Commands.RegistrarPago;

public sealed class RegistrarPagoCuotaCommandValidator : AbstractValidator<RegistrarPagoCuotaCommand>
{
    public RegistrarPagoCuotaCommandValidator()
    {
        RuleFor(x => x.CuentaPorCobrarId)
            .NotEmpty();

        RuleFor(x => x.MontoCOP)
            .GreaterThan(0);
    }
}
