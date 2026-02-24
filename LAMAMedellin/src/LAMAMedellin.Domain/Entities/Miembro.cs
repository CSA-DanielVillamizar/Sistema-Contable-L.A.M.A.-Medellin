using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

public sealed class Miembro : BaseEntity
{
    public string Nombre { get; private set; }
    public string Apellidos { get; private set; }
    public string Documento { get; private set; }
    public string Email { get; private set; }
    public string Telefono { get; private set; }
    public TipoAfiliacion TipoAfiliacion { get; private set; }
    public EstadoMiembro Estado { get; private set; }

    public Miembro(
        string nombre,
        string apellidos,
        string documento,
        string email,
        string telefono,
        TipoAfiliacion tipoAfiliacion,
        EstadoMiembro estado)
    {
        Nombre = ValidarTextoRequerido(nombre, nameof(nombre));
        Apellidos = ValidarTextoRequerido(apellidos, nameof(apellidos));
        Documento = ValidarTextoRequerido(documento, nameof(documento));
        Email = ValidarTextoRequerido(email, nameof(email));
        Telefono = ValidarTextoRequerido(telefono, nameof(telefono));
        TipoAfiliacion = tipoAfiliacion;
        Estado = estado;
    }

    public void ActualizarDatos(
        string nombre,
        string apellidos,
        string documento,
        string email,
        string telefono,
        TipoAfiliacion tipoAfiliacion,
        EstadoMiembro estado)
    {
        Nombre = ValidarTextoRequerido(nombre, nameof(nombre));
        Apellidos = ValidarTextoRequerido(apellidos, nameof(apellidos));
        Documento = ValidarTextoRequerido(documento, nameof(documento));
        Email = ValidarTextoRequerido(email, nameof(email));
        Telefono = ValidarTextoRequerido(telefono, nameof(telefono));
        TipoAfiliacion = tipoAfiliacion;
        Estado = estado;
    }

    public void Desactivar()
    {
        Estado = EstadoMiembro.Inactivo;
    }

    private static string ValidarTextoRequerido(string valor, string nombreParametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ArgumentException($"{nombreParametro} es obligatorio.", nombreParametro);
        }

        return valor.Trim();
    }
}
