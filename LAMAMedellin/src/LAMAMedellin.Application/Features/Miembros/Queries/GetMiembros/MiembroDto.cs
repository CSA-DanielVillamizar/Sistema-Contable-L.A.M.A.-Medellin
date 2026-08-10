namespace LAMAMedellin.Application.Features.Miembros.Queries.GetMiembros;

public sealed record MiembroDto(
    Guid Id,
    string? DocumentoIdentidad,
    string Nombres,
    string Apellidos,
    string Apodo,
    DateOnly? FechaIngreso,
    string Rango,
    bool EsActivo,
    string TipoSangre,
    string? NombreContactoEmergencia,
    string? TelefonoContactoEmergencia,
    string? MarcaMoto,
    string? ModeloMoto,
    string? Placa,
    int? Cilindraje);
