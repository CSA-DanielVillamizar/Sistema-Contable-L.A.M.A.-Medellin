using LAMAMedellin.Domain.Common;

namespace LAMAMedellin.Domain.Entities;

/// <summary>
/// Cuenta bancaria de la fundacion. Es la unica forma de tesoreria del sistema:
/// toda entrada y salida de dinero se recibe o se paga por transferencia, sin
/// manejo de efectivo (EPIC 05 del backlog: "todo movimiento impacta Banco, sin
/// caja").
///
/// Reemplaza a la antigua entidad Caja, que coexistia con esta y era la que
/// llevaba la cuenta contable, de modo que habia dos representaciones
/// solapadas de donde esta el dinero.
/// </summary>
public sealed class Banco : BaseEntity
{
    public string Nombre { get; private set; }
    public string NumeroCuenta { get; private set; }
    public decimal SaldoActual { get; private set; }

    /// <summary>
    /// Cuenta contable que representa este banco en el libro. Es obligatoria:
    /// sin ella los movimientos de tesoreria no tendrian contrapartida y la
    /// partida doble quedaria incompleta.
    /// </summary>
    public Guid CuentaContableId { get; private set; }

    /// <summary>
    /// Una cuenta inactiva conserva su historia y su saldo pero no admite
    /// movimientos nuevos. Permite dar de baja una cuenta sin borrar los
    /// movimientos ya registrados contra ella.
    /// </summary>
    public bool EsActivo { get; private set; }

    public CuentaContable? CuentaContable { get; private set; }

#pragma warning disable CS8618
    private Banco() { }
#pragma warning restore CS8618

    public Banco(string nombre, string numeroCuenta, decimal saldoActual, Guid cuentaContableId, bool esActivo = true)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("Nombre es obligatorio.", nameof(nombre));
        }

        if (string.IsNullOrWhiteSpace(numeroCuenta))
        {
            throw new ArgumentException("NumeroCuenta es obligatorio.", nameof(numeroCuenta));
        }

        if (cuentaContableId == Guid.Empty)
        {
            throw new ArgumentException("CuentaContableId es obligatorio.", nameof(cuentaContableId));
        }

        Nombre = nombre.Trim();
        NumeroCuenta = numeroCuenta.Trim();
        SaldoActual = saldoActual;
        CuentaContableId = cuentaContableId;
        EsActivo = esActivo;
    }

    public void AplicarIngreso(decimal monto)
    {
        ValidarMovimiento(monto);
        SaldoActual += monto;
    }

    public void AplicarEgreso(decimal monto)
    {
        ValidarMovimiento(monto);

        if (SaldoActual < monto)
        {
            throw new ReglaNegocioException("Saldo insuficiente en la cuenta bancaria para registrar el egreso.");
        }

        SaldoActual -= monto;
    }

    public void Desactivar() => EsActivo = false;

    public void Activar() => EsActivo = true;

    private void ValidarMovimiento(decimal monto)
    {
        if (!EsActivo)
        {
            throw new ReglaNegocioException($"La cuenta bancaria '{Nombre}' esta inactiva y no admite movimientos.");
        }

        if (monto <= 0)
        {
            throw new ReglaNegocioException("El monto debe ser mayor a cero.");
        }
    }
}
