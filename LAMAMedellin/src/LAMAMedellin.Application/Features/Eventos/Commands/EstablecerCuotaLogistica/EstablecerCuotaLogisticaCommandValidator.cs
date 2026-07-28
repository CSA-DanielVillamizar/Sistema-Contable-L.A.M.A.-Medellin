using FluentValidation;

namespace LAMAMedellin.Application.Features.Eventos.Commands.EstablecerCuotaLogistica;

public sealed class EstablecerCuotaLogisticaCommandValidator : AbstractValidator<EstablecerCuotaLogisticaCommand>
{
    public EstablecerCuotaLogisticaCommandValidator()
    {
        RuleFor(x => x.EventoId)
            .NotEmpty()
            .WithMessage("EventoId es obligatorio.");

        RuleFor(x => x.CuotaLogisticaCOP)
            .GreaterThan(0)
            .When(x => x.CuotaLogisticaCOP.HasValue)
            .WithMessage("CuotaLogisticaCOP debe ser mayor a cero.");
    }
}
