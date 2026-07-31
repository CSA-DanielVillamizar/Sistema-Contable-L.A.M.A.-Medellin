namespace LAMAMedellin.Application.Features.Transacciones.Queries.GetCatalogoBancos;

/// <summary>
/// El DTO solo exponia el numero de cuenta, asi que los desplegables de
/// ingresos, egresos y donaciones mostraban cadenas como
/// "MIGRADO-72abf620-..." en vez del nombre de la cuenta. Se agrega Nombre,
/// que es lo unico que el usuario puede reconocer.
/// </summary>
public sealed record CatalogoBancoDto(Guid Id, string Nombre, string NumeroCuenta);
