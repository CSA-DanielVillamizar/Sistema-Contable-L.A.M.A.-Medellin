using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

/// <summary>
/// Que cuenta contable usa cada operacion del negocio (historia 1-2).
///
/// Saca del codigo una decision que es del contador, no del programador. La
/// pista de auditoria la aporta BaseEntity: queda registrado quien cambio el
/// mapeo y cuando, que es justamente lo que pide el criterio de la historia.
/// </summary>
public sealed class MapeoContable : BaseEntity
{
    public TipoOperacionContable TipoOperacion { get; private set; }

    public Guid CuentaContableId { get; private set; }

    public CuentaContable? CuentaContable { get; private set; }

#pragma warning disable CS8618
    private MapeoContable() { }
#pragma warning restore CS8618

    public MapeoContable(TipoOperacionContable tipoOperacion, Guid cuentaContableId)
    {
        if (cuentaContableId == Guid.Empty)
        {
            throw new ReglaNegocioException("CuentaContableId es obligatorio.");
        }

        TipoOperacion = tipoOperacion;
        CuentaContableId = cuentaContableId;
    }

    public void Reasignar(Guid cuentaContableId)
    {
        if (cuentaContableId == Guid.Empty)
        {
            throw new ReglaNegocioException("CuentaContableId es obligatorio.");
        }

        CuentaContableId = cuentaContableId;
    }
}
