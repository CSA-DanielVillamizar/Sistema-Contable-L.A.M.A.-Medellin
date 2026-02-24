using FluentValidation;

namespace LAMAMedellin.Application.Features.Miembros.Commands.UpdateMiembro;

public sealed class UpdateMiembroCommandValidator : AbstractValidator<UpdateMiembroCommand>
{
    public UpdateMiembroCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Apellidos)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(x => x.Documento)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(200);

        RuleFor(x => x.Telefono)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.TipoAfiliacion)
            .IsInEnum();

        RuleFor(x => x.Estado)
            .IsInEnum();
    }
}
