using FluentValidation;

namespace LAMAMedellin.Application.Features.Cartera.Commands.CrearMiembro;

public sealed class CrearMiembroCommandValidator : AbstractValidator<CrearMiembroCommand>
{
    public CrearMiembroCommandValidator()
    {
        RuleFor(x => x.DocumentoIdentidad)
            .NotEmpty().WithMessage("DocumentoIdentidad es obligatorio.")
            .MaximumLength(50).WithMessage("DocumentoIdentidad no puede exceder 50 caracteres.");

        RuleFor(x => x.Nombres)
            .NotEmpty().WithMessage("Nombres es obligatorio.")
            .MaximumLength(150).WithMessage("Nombres no puede exceder 150 caracteres.");

        RuleFor(x => x.Apellidos)
            .NotEmpty().WithMessage("Apellidos es obligatorio.")
            .MaximumLength(150).WithMessage("Apellidos no puede exceder 150 caracteres.");

        RuleFor(x => x.Apodo)
            .MaximumLength(100).WithMessage("Apodo no puede exceder 100 caracteres.");

        RuleFor(x => x.FechaIngreso)
            .NotEmpty().WithMessage("FechaIngreso es obligatoria.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("FechaIngreso no puede ser mayor a hoy.");

        RuleFor(x => x.TipoSangre)
            .IsInEnum().WithMessage("TipoSangre debe ser un valor válido.");

        RuleFor(x => x.NombreContactoEmergencia)
            .NotEmpty().WithMessage("NombreContactoEmergencia es obligatorio.")
            .MaximumLength(150).WithMessage("NombreContactoEmergencia no puede exceder 150 caracteres.");

        RuleFor(x => x.TelefonoContactoEmergencia)
            .NotEmpty().WithMessage("TelefonoContactoEmergencia es obligatorio.")
            .MaximumLength(30).WithMessage("TelefonoContactoEmergencia no puede exceder 30 caracteres.");

        RuleFor(x => x.MarcaMoto)
            .NotEmpty().WithMessage("MarcaMoto es obligatorio.")
            .MaximumLength(100).WithMessage("MarcaMoto no puede exceder 100 caracteres.");

        RuleFor(x => x.ModeloMoto)
            .NotEmpty().WithMessage("ModeloMoto es obligatorio.")
            .MaximumLength(100).WithMessage("ModeloMoto no puede exceder 100 caracteres.");

        RuleFor(x => x.Cilindraje)
            .GreaterThan(0).WithMessage("Cilindraje debe ser mayor que cero.");

        RuleFor(x => x.Placa)
            .NotEmpty().WithMessage("Placa es obligatoria.")
            .MaximumLength(20).WithMessage("Placa no puede exceder 20 caracteres.");

        RuleFor(x => x.Rango)
            .IsInEnum().WithMessage("Rango debe ser un valor válido.")
            .When(x => x.Rango is not null);
    }
}
