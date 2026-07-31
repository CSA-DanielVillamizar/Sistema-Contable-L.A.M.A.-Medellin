namespace LAMAMedellin.Domain.Enums;

/// <summary>
/// Espeja EstadoCuentaPorCobrar: la obligacion con un proveedor recorre el
/// mismo ciclo que la de un miembro, solo que en sentido contrario.
///
/// Los valores numericos son datos persistidos: agregar siempre con el
/// siguiente numero libre, nunca reasignar los existentes.
/// </summary>
public enum EstadoCuentaPorPagar
{
    Pendiente = 1,
    PagadaParcial = 2,
    Pagada = 3,
    Anulada = 4,
}
