namespace LAMAMedellin.Application.Common.Interfaces.Services;

/// <summary>
/// Identifica a quien esta ejecutando la operacion en curso, para la pista de
/// auditoria. Devuelve null cuando no hay usuario asociado, por ejemplo en la
/// siembra inicial o en tareas de arranque.
/// </summary>
public interface IUsuarioActual
{
    /// <summary>
    /// Identificador legible del usuario. Se prefiere el correo por ser lo que
    /// un auditor puede interpretar sin consultar otra tabla; si el token no lo
    /// trae, cae al object id de Entra.
    /// </summary>
    string? Identificador { get; }
}
