using LAMAMedellin.Application.Features.Contabilidad.Queries.GetBalancePrueba;
using LAMAMedellin.Application.Features.Contabilidad.Queries.GetLibroDiario;
using LAMAMedellin.Application.Features.Contabilidad.Queries.GetLibroMayor;
using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

/// <summary>
/// Consultas de los libros oficiales. Todas leen unicamente comprobantes
/// asentados: incluir borradores o anulados daria cifras que no corresponden a
/// la contabilidad.
/// </summary>
public interface ILibrosContablesRepository
{
    Task<IReadOnlyList<MovimientoLibroDiarioDto>> GetLibroDiarioAsync(
        DateOnly desde,
        DateOnly hasta,
        Guid? centroCostoId,
        CancellationToken cancellationToken = default);

    Task<CuentaContable?> GetCuentaAsync(Guid cuentaContableId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MovimientoLibroMayorDto>> GetMovimientosMayorAsync(
        Guid cuentaContableId,
        DateOnly desde,
        DateOnly hasta,
        Guid? centroCostoId,
        CancellationToken cancellationToken = default);

    /// <summary>Debe y haber acumulados de una cuenta ANTES de la fecha indicada.</summary>
    Task<(decimal Debe, decimal Haber)> GetAcumuladoAnteriorAsync(
        Guid cuentaContableId,
        DateOnly desde,
        Guid? centroCostoId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SaldoCuentaBalanceDto>> GetBalancePruebaAsync(
        int anio,
        int mes,
        Guid? centroCostoId,
        CancellationToken cancellationToken = default);
}
