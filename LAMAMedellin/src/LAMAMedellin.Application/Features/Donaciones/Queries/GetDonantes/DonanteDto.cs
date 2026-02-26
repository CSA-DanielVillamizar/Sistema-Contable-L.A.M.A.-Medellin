namespace LAMAMedellin.Application.Features.Donaciones.Queries.GetDonantes;

public sealed record DonanteDto(
    Guid Id,
    string NombreORazonSocial,
    string TipoDocumento,
    string NumeroDocumento,
    string Email,
    string TipoPersona);
