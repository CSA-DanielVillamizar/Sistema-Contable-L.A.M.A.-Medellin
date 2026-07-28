using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetLibroDiario;

public sealed class GetLibroDiarioQueryHandler(ILibrosContablesRepository librosRepository)
    : IRequestHandler<GetLibroDiarioQuery, LibroDiarioDto>
{
    public async Task<LibroDiarioDto> Handle(GetLibroDiarioQuery request, CancellationToken cancellationToken)
    {
        var movimientos = await librosRepository.GetLibroDiarioAsync(
            request.Desde,
            request.Hasta,
            request.CentroCostoId,
            cancellationToken);

        var totalDebe = movimientos.Sum(m => m.Debe);
        var totalHaber = movimientos.Sum(m => m.Haber);

        return new LibroDiarioDto(
            request.Desde,
            request.Hasta,
            totalDebe,
            totalHaber,
            // Sobre el conjunto completo de asientos los totales deben coincidir.
            // Si no coinciden hay un comprobante descuadrado en el libro.
            totalDebe == totalHaber,
            movimientos);
    }
}
