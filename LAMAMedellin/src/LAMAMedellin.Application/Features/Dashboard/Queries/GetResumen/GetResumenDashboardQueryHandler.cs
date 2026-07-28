using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Dashboard.Queries.GetResumen;

public sealed class GetResumenDashboardQueryHandler(
    IMiembroRepository miembroRepository,
    IBancoRepository bancoRepository,
    IEventoRepository eventoRepository)
    : IRequestHandler<GetResumenDashboardQuery, DashboardResumenDto>
{
    public async Task<DashboardResumenDto> Handle(
        GetResumenDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var totalMiembrosActivos = await miembroRepository.CountActivosAsync(cancellationToken);
        var totalDineroBancos = await bancoRepository.GetTotalSaldoActualAsync(cancellationToken);
        var proximoEvento = await eventoRepository.GetProximoProgramadoAsync(cancellationToken);

        // Defensa ante repositorios vacios o valores no inicializados.
        var totalMiembrosActivosSeguro = totalMiembrosActivos < 0 ? 0 : totalMiembrosActivos;
        var totalDineroBancosSeguro = totalDineroBancos;
        var proximoEventoNombre = proximoEvento?.Nombre;
        var proximaFechaEvento = proximoEvento?.FechaProgramada;

        return new DashboardResumenDto(
            totalMiembrosActivosSeguro,
            totalDineroBancosSeguro,
            proximoEventoNombre,
            proximaFechaEvento);
    }
}
