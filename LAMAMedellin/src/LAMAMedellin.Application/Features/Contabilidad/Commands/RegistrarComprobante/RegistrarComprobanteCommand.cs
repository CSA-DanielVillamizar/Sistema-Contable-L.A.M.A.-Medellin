using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Commands.RegistrarComprobante;

public sealed record RegistrarComprobanteCommand(
    DateTime Fecha,
    TipoComprobante Tipo,
    string Descripcion,
    IReadOnlyList<RegistrarAsientoComprobanteDto> Asientos) : IRequest<Guid>;

public sealed record RegistrarAsientoComprobanteDto(
    Guid CuentaContableId,
    Guid? TerceroId,
    Guid CentroCostoId,
    decimal Debe,
    decimal Haber,
    string Referencia);
