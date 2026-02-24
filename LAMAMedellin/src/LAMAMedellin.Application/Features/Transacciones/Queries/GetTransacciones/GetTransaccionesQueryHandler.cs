using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Transacciones.Queries.GetTransacciones;

public sealed class GetTransaccionesQueryHandler(
    ITransaccionRepository transaccionRepository)
    : IRequestHandler<GetTransaccionesQuery, List<TransaccionDto>>
{
    public async Task<List<TransaccionDto>> Handle(
        GetTransaccionesQuery request,
        CancellationToken cancellationToken)
    {
        var transacciones = await transaccionRepository.GetAllWithDetallesAsync(cancellationToken);

        return transacciones
            .Select(transaccion => new TransaccionDto(
                transaccion.Id,
                transaccion.Fecha,
                transaccion.Tipo.ToString(),
                transaccion.MontoCOP,
                transaccion.Descripcion,
                transaccion.CentroCosto?.Nombre ?? string.Empty,
                transaccion.Banco?.NumeroCuenta ?? string.Empty))
            .ToList();
    }
}
