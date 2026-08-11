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

    /// <summary>
    /// Obsoleto: no distinguia la etapa dentro de la progresion Prospect -&gt;
    /// Rockets -&gt; Full Color de Dama L.A.M.A. Se deja definido porque el
    /// numero es un dato persistido y nunca se reasigna, pero los miembros
    /// nuevos usan <see cref="DamaProspect"/>, <see cref="DamaRockets"/> o
    /// <see cref="DamaFullColor"/> segun su etapa real.
    /// </summary>
    LadyLama = 6,

    /// <summary>
    /// Hijos menores de un miembro (ej. Amelia Villamizar en el capitulo
    /// Medellin). No paga cuota mensual, pero el capitulo gestiona a su
    /// nombre la renovacion anual de membresia internacional (cuenta
    /// 281505 - Renovacion Membresia Internacional L.A.M.A.), igual que
    /// para el resto de miembros con esa renovacion.
    /// </summary>
    Youth = 7,

    /// <summary>
    /// Dama L.A.M.A. (reconocimiento para las Damas que manejan su propia
    /// moto) en su etapa de Prospect. Reemplaza el uso de LadyLama para
    /// datos nuevos: LadyLama se deja definido (nunca se reasigna un
    /// numero persistido) pero deja de usarse porque no distinguia la
    /// etapa dentro de la progresion Prospect -> Rockets -> Full Color,
    /// igual que el resto de miembros.
    /// </summary>
    DamaProspect = 8,

    /// <summary>Dama L.A.M.A. en su etapa de Rockets.</summary>
    DamaRockets = 9,

    /// <summary>Dama L.A.M.A. en su etapa de Full Color Member.</summary>
    DamaFullColor = 10,

    /// <summary>
    /// Miembro honorario. No paga cuota mensual ni la renovacion anual de
    /// membresia internacional.
    /// </summary>
    Honorary = 11,
}

public static class TipoAfiliacionExtensions
{
    /// <summary>
    /// Quien esta exento de la cuota mensual. Spousal, Youth y Honorary,
    /// confirmado por el capitulo. Se expresa aqui y no como una tarifa en
    /// cero para que la exencion sea una regla explicita y no un valor que
    /// alguien pueda editar sin darse cuenta de lo que significa.
    /// </summary>
    public static bool ExentoDeCuotaMensual(this TipoAfiliacion tipo) =>
        tipo is TipoAfiliacion.Esposa or TipoAfiliacion.Youth or TipoAfiliacion.Honorary;

    /// <summary>
    /// Quien esta exento de la renovacion anual de membresia internacional
    /// (cuenta 281505). Solo Honorary: el resto de tipos, incluidos Spousal,
    /// Youth y las tres etapas de Dama L.A.M.A., si la pagan (ver
    /// GenerarRenovacionAnualCommandHandler).
    /// </summary>
    public static bool ExentoDeRenovacionAnual(this TipoAfiliacion tipo) =>
        tipo is TipoAfiliacion.Honorary;
}
