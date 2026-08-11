namespace LAMAMedellin.Application.Features.Miembros.Queries.GetMiembroById;

public sealed record MiembroDto(
    Guid Id,
    string? DocumentoIdentidad,
    string Nombres,
    string Apellidos,
    string Apodo,
    DateOnly? FechaIngreso,
    string? Rango,
    string TipoAfiliacion,
    bool EsActivo,
    string TipoSangre,
    string? NombreContactoEmergencia,
    string? TelefonoContactoEmergencia,
    string? MarcaMoto,
    string? ModeloMoto,
    int? Cilindraje,
    string? Placa);
