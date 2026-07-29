using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface ICuentaPorCobrarRepository
{
    /// <summary>
    /// Idempotencia de la generacion mensual: consulta por miembro, concepto Y
    /// periodo. El metodo anterior recibia el periodo y lo ignoraba, de modo que
    /// bastaba una sola cuenta por cobrar para que el miembro nunca volviera a
    /// recibir obligaciones.
    /// </summary>
    Task<bool> ExisteParaMiembroYPeriodoAsync(
        Guid miembroId,
        Guid conceptoCobroId,
        string periodo,
        CancellationToken cancellationToken = default);

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
