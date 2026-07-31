using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetComprobantes;

public sealed class GetComprobantesQueryHandler(IComprobanteRepository comprobanteRepository)
    : IRequestHandler<GetComprobantesQuery, IReadOnlyList<ComprobanteResumenDto>>
{
    public async Task<IReadOnlyList<ComprobanteResumenDto>> Handle(
        GetComprobantesQuery request,
        CancellationToken cancellationToken)
    {
        var comprobantes = await comprobanteRepository.GetRecientesAsync(request.Limite, cancellationToken);

        return comprobantes
            .Select(c => new ComprobanteResumenDto(
                c.Id,
                c.NumeroConsecutivo,
                c.Fecha,
                c.TipoComprobante.ToString(),
                c.Descripcion,
                c.EstadoComprobante.ToString(),
                // El total de un comprobante cuadrado es la suma del debe; da
                // igual cual de los dos lados se sume.
                c.AsientosContables.Sum(a => a.Debe)))
            .ToList();
    }
}
