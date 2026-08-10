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

    /// <summary>
    /// Hijos menores de un miembro (ej. Amelia Villamizar en el capitulo
    /// Medellin). No paga cuota mensual, pero el capitulo gestiona a su
    /// nombre la renovacion anual de membresia internacional (cuenta
    /// 281505 - Renovacion Membresia Internacional L.A.M.A.), igual que
    /// para el resto de miembros con esa renovacion.
    /// </summary>
    Youth = 7,
}

public static class TipoAfiliacionExtensions
{
    /// <summary>
    /// Quien esta exento de la cuota mensual. Spousal y Youth, confirmado por
    /// el capitulo. Se expresa aqui y no como una tarifa en cero para que la
    /// exencion sea una regla explicita y no un valor que alguien pueda editar
    /// sin darse cuenta de lo que significa.
    /// </summary>
    public static bool ExentoDeCuotaMensual(this TipoAfiliacion tipo) =>
        tipo is TipoAfiliacion.Esposa or TipoAfiliacion.Youth;
}
