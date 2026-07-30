namespace LAMAMedellin.Domain.Enums;

/// <summary>
/// Operaciones del negocio que necesitan una cuenta contable asociada
/// (historia 1-2). Son las que enumera el backlog como minimas.
///
/// Existe para sacar del codigo la decision de que cuenta usa cada operacion.
/// Antes esa relacion estaba repartida: cada concepto de cobro y cada producto
/// llevaba la suya, y las de diferencia en cambio o gastos bancarios
/// simplemente no tenian donde declararse.
///
/// Los valores numericos son datos persistidos: agregar siempre con el
/// siguiente numero libre, nunca reasignar los existentes.
/// </summary>
public enum TipoOperacionContable
{
    IngresoCuotas = 1,
    IngresoDonaciones = 2,
    IngresoMerchandising = 3,

    /// <summary>Ganancia cuando la tasa de liquidacion favorece al capitulo.</summary>
    IngresoDiferenciaCambio = 4,

    /// <summary>Perdida cuando la tasa de liquidacion es desfavorable.</summary>
    GastoDiferenciaCambio = 5,

    GastoAdministrativo = 6,
    GastoOperativo = 7,
    GastoEventos = 8,
    GastoProyectos = 9,
    GastoBancario = 10,
    CompraInventario = 11,
}
