using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetComprobantes;

/// <summary>
/// Listado de comprobantes. No existia: se podian crear pero no consultar, de
/// modo que ninguna pantalla podia ofrecerlos para elegir.
/// </summary>
public sealed record GetComprobantesQuery(int Limite = 200) : IRequest<IReadOnlyList<ComprobanteResumenDto>>;

public sealed record ComprobanteResumenDto(
    Guid Id,
    string NumeroConsecutivo,
    DateTime Fecha,
    string TipoComprobante,
    string Descripcion,
    string Estado,
    decimal Total);
