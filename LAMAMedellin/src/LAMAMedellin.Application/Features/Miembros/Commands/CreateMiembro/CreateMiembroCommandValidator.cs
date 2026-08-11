using FluentValidation;

namespace LAMAMedellin.Application.Features.Miembros.Commands.CreateMiembro;

public sealed class CreateMiembroCommandValidator : AbstractValidator<CreateMiembroCommand>
{
    public CreateMiembroCommandValidator()
    {
        RuleFor(x => x.DocumentoIdentidad)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Nombres)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Apellidos)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Apodo)
            .MaximumLength(100);

        RuleFor(x => x.FechaIngreso)
            .NotEmpty()
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));

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
