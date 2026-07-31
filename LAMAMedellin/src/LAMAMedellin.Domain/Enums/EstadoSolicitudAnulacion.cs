namespace LAMAMedellin.Domain.Enums;

/// <summary>
/// Los valores numericos son datos persistidos: agregar siempre con el
/// siguiente numero libre, nunca reasignar los existentes.
/// </summary>
public enum EstadoSolicitudAnulacion
{
    Pendiente = 1,
    Aprobada = 2,
    Rechazada = 3,
}
