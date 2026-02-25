using FluentValidation;

namespace LAMAMedellin.Application.Features.Donaciones.Commands.CrearDonante;

public sealed class CrearDonanteCommandValidator : AbstractValidator<CrearDonanteCommand>
{
    public CrearDonanteCommandValidator()
    {
        RuleFor(x => x.NombreORazonSocial)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.TipoDocumento)
            .IsInEnum();

        RuleFor(x => x.NumeroDocumento)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(200);

        RuleFor(x => x.TipoPersona)
            .IsInEnum();
    }
}
