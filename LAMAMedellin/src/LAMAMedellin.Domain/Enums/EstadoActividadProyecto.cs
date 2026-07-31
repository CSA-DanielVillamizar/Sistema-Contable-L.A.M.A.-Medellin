namespace LAMAMedellin.Domain.Enums;

/// <summary>
/// Los valores numericos son datos persistidos: agregar siempre con el
/// siguiente numero libre, nunca reasignar los existentes.
/// </summary>
public enum EstadoActividadProyecto
{
    Planificada = 1,
    EnEjecucion = 2,
    Completada = 3,
    Cancelada = 4,
}
