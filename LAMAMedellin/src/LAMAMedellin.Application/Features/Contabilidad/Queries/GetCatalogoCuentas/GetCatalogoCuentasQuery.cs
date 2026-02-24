using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetCatalogoCuentas;

/// <summary>
/// Retorna el listado completo del Catálogo de Cuentas ordenado por código.
/// Si <see cref="SoloAsentables"/> es true, filtra únicamente las cuentas
/// que admiten asientos contables (PermiteMovimiento = true).
/// </summary>
public sealed record GetCatalogoCuentasQuery(bool SoloAsentables = false)
    : IRequest<List<CuentaContableDto>>;
