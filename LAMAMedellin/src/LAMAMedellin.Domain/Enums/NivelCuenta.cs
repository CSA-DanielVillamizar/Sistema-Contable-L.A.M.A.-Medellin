namespace LAMAMedellin.Domain.Enums;

/// <summary>
/// Nivel jerárquico en el Catálogo de Cuentas (PUC) derivado de la longitud del código numérico.
/// Clase=1 dígito, Grupo=2 dígitos, Cuenta=4 dígitos, Subcuenta=6 dígitos, Auxiliar=8+ dígitos.
/// </summary>
public enum NivelCuenta
{
    Clase = 1,
    Grupo = 2,
    Cuenta = 3,
    Subcuenta = 4,
    Auxiliar = 5
}
