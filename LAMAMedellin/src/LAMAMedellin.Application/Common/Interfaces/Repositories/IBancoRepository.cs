using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface IBancoRepository
{
    Task<List<Banco>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Banco?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Banco banco, CancellationToken cancellationToken = default);

    /// <summary>
    /// El numero de cuenta identifica la cuenta ante el banco: repetirlo haria
    /// que dos registros compitan por los mismos movimientos de extracto.
    /// </summary>
    Task<bool> ExisteNumeroCuentaAsync(string numeroCuenta, Guid? excluyendoId = null, CancellationToken cancellationToken = default);

    /// <summary>Primera cuenta bancaria activa. Hoy la operacion usa una sola.</summary>
    Task<Banco?> GetDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>Saldo consolidado de las cuentas activas, para el tablero.</summary>
    Task<decimal> GetTotalSaldoActualAsync(CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
