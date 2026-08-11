using FluentValidation;

namespace LAMAMedellin.Application.Features.Miembros.Commands.UpdateMiembro;

public sealed class UpdateMiembroCommandValidator : AbstractValidator<UpdateMiembroCommand>
{
    public UpdateMiembroCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.TipoSangre)
            .IsInEnum();

        RuleFor(x => x.NombreContactoEmergencia)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.TelefonoContactoEmergencia)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.MarcaMoto)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.ModeloMoto)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Cilindraje)
            .GreaterThan(0);

        RuleFor(x => x.Placa)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.TipoAfiliacion)
            .IsInEnum();

        RuleFor(x => x.Rango)
            .IsInEnum()
            .When(x => x.Rango is not null);
    }
}
