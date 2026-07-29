namespace LAMAMedellin.Domain.Enums;

/// <summary>
/// Tipo de afiliacion del miembro. Determina si paga cuota y a que centro de
/// costo se imputa.
///
/// Los valores numericos son datos persistidos: agregar siempre con el
/// siguiente numero libre, nunca reasignar los existentes.
/// </summary>
public enum TipoAfiliacion
{
    FullColor = 1,
    Rockets = 2,
    Prospect = 3,

    /// <summary>Spousal. No paga cuota mensual.</summary>
    Esposa = 4,

    Asociado = 5,

    /// <summary>Lady L.A.M.A. Paga cuota como el resto de miembros.</summary>
    LadyLama = 6,
}

public static class TipoAfiliacionExtensions
{
    /// <summary>
    /// Quien esta exento de la cuota mensual. Hoy solo Spousal, confirmado por
    /// el capitulo. Se expresa aqui y no como una tarifa en cero para que la
    /// exencion sea una regla explicita y no un valor que alguien pueda editar
    /// sin darse cuenta de lo que significa.
    /// </summary>
    public static bool ExentoDeCuotaMensual(this TipoAfiliacion tipo) =>
        tipo == TipoAfiliacion.Esposa;
}
