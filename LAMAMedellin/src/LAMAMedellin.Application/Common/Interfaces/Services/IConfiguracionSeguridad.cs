namespace LAMAMedellin.Application.Common.Interfaces.Services;

/// <summary>
/// Decisiones de seguridad que dependen del despliegue y no del codigo.
/// </summary>
public interface IConfiguracionSeguridad
{
    /// <summary>
    /// Si un correo figura en la lista de administradores iniciales.
    ///
    /// La lista vive en configuracion, no en el codigo: quien administra el
    /// capitulo cambia con el tiempo, y tener nombres propios compilados
    /// obligaria a un despliegue cada vez que eso pasa. Ademas permite que cada
    /// entorno tenga los suyos.
    /// </summary>
    bool EsAdministradorInicial(string? email);
}
