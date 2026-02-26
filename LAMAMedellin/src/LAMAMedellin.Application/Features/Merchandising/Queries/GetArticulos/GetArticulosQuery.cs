using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Queries.GetArticulos;

public sealed record GetArticulosQuery : IRequest<IReadOnlyList<ArticuloDto>>;

public sealed record ArticuloDto(
    Guid Id,
    string Nombre,
    string SKU,
    string Descripcion,
    string Categoria,
    decimal PrecioVenta,
    decimal CostoPromedio,
    int StockActual,
    Guid CuentaContableIngresoId);

public sealed class GetArticulosQueryHandler(IArticuloRepository articuloRepository)
    : IRequestHandler<GetArticulosQuery, IReadOnlyList<ArticuloDto>>
{
    public async Task<IReadOnlyList<ArticuloDto>> Handle(GetArticulosQuery request, CancellationToken cancellationToken)
    {
        var articulos = await articuloRepository.GetAllAsync(cancellationToken);

        return articulos
            .Select(a => new ArticuloDto(
                a.Id,
                a.Nombre,
                a.SKU,
                a.Descripcion,
                a.Categoria.ToString(),
                a.PrecioVenta,
                a.CostoPromedio,
                a.StockActual,
                a.CuentaContableIngresoId))
            .ToList();
    }
}
