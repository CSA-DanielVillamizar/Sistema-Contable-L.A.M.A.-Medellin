namespace LAMAMedellin.Domain.Enums;

/// <summary>
/// Cargo directivo del miembro dentro del capitulo. No es un nivel de
/// progresion (eso lo cubre TipoAfiliacion con Prospect/Rockets/Full Color):
/// es opcional y solo lo tienen quienes ocupan una posicion de la directiva.
///
/// Los valores numericos son datos persistidos: agregar siempre con el
/// siguiente numero libre, nunca reasignar los existentes.
/// </summary>
public enum RangoClub
{
    President = 1,
    VicePresident = 2,
    Treasurer = 3,
    BusinessManager = 4,
    Secretary = 5,

    /// <summary>Moto Touring Officer (MTO).</summary>
    MotoTouringOfficer = 6,

    SergeantAtArms = 7,
    RoadCaptain = 8,
}
