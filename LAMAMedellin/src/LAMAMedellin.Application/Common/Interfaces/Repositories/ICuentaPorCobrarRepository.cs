using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface ICuentaPorCobrarRepository
{
    Task<bool> ExistePorMiembroYPeriodoAsync(Guid miembroId, string periodo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cuentas con saldo por cobrar, sin importar si el saldo esta intacto o ya
    /// recibio abonos parciales. Filtrar por Estado == Pendiente dejaba fuera las
    /// PagadaParcial, que siguen debiendo dinero.
    /// </summary>
    Task<List<CuentaPorCobrar>> GetPendientesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CuentaPorCobrar>> GetCarteraEnMoraAsync(
        DateOnly fechaCorte,
        CancellationToken cancellationToken = default);
    Task<List<CuentaPorCobrar>> GetListadoAsync(
        EstadoCuentaPorCobrar? estado = null,
        Guid? miembroId = null,
        CancellationToken cancellationToken = default);
    Task<CuentaPorCobrar?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(CuentaPorCobrar cuentaPorCobrar, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<CuentaPorCobrar> cuentasPorCobrar, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
