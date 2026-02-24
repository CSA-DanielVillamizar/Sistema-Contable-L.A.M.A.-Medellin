namespace LAMAMedellin.Application.Features.Miembros.Queries.GetMiembros;

public sealed record MiembroDto(
    Guid Id,
    string NombreCompleto,
    string Documento,
    string Email,
    string Telefono,
    string TipoAfiliacion,
    string Estado);
