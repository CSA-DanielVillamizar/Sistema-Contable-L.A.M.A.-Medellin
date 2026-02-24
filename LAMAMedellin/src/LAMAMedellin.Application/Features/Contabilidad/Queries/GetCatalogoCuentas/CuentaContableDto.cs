namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetCatalogoCuentas;

/// <summary>
/// Representación plana de una cuenta del Catálogo de Cuentas (PUC ESAL).
/// El campo <see cref="Nivel"/> es derivado de la longitud del código.
/// </summary>
public sealed record CuentaContableDto(
    Guid Id,
    string Codigo,
    string Descripcion,
    int Nivel,
    string NivelNombre,
    string NaturalezaNombre,
    bool PermiteMovimiento,
    bool ExigeTercero,
    Guid? CuentaPadreId);
