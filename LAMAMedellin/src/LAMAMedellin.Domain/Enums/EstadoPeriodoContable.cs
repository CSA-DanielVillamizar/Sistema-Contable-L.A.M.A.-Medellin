namespace LAMAMedellin.Domain.Enums;

/// <summary>
/// Ciclo de vida de un periodo contable mensual.
/// El avance es en un solo sentido: Abierto -> ValidadoTesoreria -> Cerrado.
/// No existe reapertura a proposito; lo posterior al cierre se corrige con
/// comprobantes de ajuste, no reabriendo el mes.
/// </summary>
public enum EstadoPeriodoContable
{
    Abierto = 1,
    ValidadoTesoreria = 2,
    Cerrado = 3,
}
