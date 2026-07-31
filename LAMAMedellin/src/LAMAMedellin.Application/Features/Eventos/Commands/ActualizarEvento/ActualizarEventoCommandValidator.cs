using FluentValidation;

namespace LAMAMedellin.Application.Features.Eventos.Commands.ActualizarEvento;

public sealed class ActualizarEventoCommandValidator : AbstractValidator<ActualizarEventoCommand>
{
    public ActualizarEventoCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.LugarEncuentro).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Destino).MaximumLength(200);
        RuleFor(x => x.TipoEvento).IsInEnum();
    }
}
