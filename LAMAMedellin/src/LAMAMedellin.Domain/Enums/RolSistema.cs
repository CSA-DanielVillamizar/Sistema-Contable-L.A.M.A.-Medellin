namespace LAMAMedellin.Domain.Enums;

/// <summary>
/// Roles internos del sistema. Son la unica fuente de autorizacion: se
/// proyectan como claims en cada peticion autenticada, de modo que se
/// administran desde la aplicacion y no desde Entra ID, tal como pide EPIC 01.
///
/// IMPORTANTE: los valores numericos son datos persistidos. No se pueden
/// reasignar ni reordenar sin migrar las filas existentes; cambiar el numero de
/// un miembro reinterpreta en silencio a los usuarios ya guardados. Para
/// agregar un rol nuevo, usar el siguiente numero libre.
/// </summary>
public enum RolSistema
{
    // --- Valores originales: los numeros se conservan tal cual. ---
    // "Administrador" se renombro a "Admin" para que el nombre del miembro
    // coincida con el que exigen los controladores. El numero sigue siendo 1,
    // asi que ninguna fila existente cambia de significado.
    Admin = 1,
    Tesorero = 2,
    Logistica = 3,
    CapitanRuta = 4,

    // --- Roles del backlog que faltaban (EPIC 01). ---
    Contador = 5,
    Operador = 6,
    Junta = 7,

    // Usado por MerchandisingController. No figura en el backlog; se conserva
    // para no alterar quien accede hoy al modulo.
    Inventario = 8,
}
