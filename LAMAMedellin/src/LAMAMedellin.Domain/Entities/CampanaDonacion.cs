using LAMAMedellin.Domain.Common;

namespace LAMAMedellin.Domain.Entities;

/// <summary>
/// Campana de donacion con meta y vigencia (historia 2-1).
///
/// Agrupa donaciones bajo un proposito y una ventana de tiempo, que es lo que
/// permite decir cuanto se recaudo de lo que se pretendia. Sin ella, todas las
/// donaciones caian en un mismo saco y no habia forma de rendir cuentas de una
/// convocatoria concreta.
///
/// La meta no limita: recaudar por encima es un buen resultado, no un error.
/// </summary>
public sealed class CampanaDonacion : BaseEntity
{
    public string Nombre { get; private set; }

    public string Descripcion { get; private set; }

    /// <summary>Cuanto se pretende recaudar. Es una referencia, no un tope.</summary>
    public decimal MetaCOP { get; private set; }

    public DateOnly FechaInicio { get; private set; }
    public DateOnly FechaFin { get; private set; }

    /// <summary>
    /// Una campana cerrada conserva sus donaciones pero ya no admite nuevas.
    /// Se prefiere sobre el borrado porque lo recaudado debe seguir existiendo.
    /// </summary>
    public bool EstaActiva { get; private set; }

    public List<Donacion> Donaciones { get; private set; } = [];

#pragma warning disable CS8618
    private CampanaDonacion() { }
#pragma warning restore CS8618

    public CampanaDonacion(
        string nombre,
        string descripcion,
        decimal metaCOP,
        DateOnly fechaInicio,
        DateOnly fechaFin)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ReglaNegocioException("El nombre de la campana es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(descripcion))
        {
            throw new ReglaNegocioException("La descripcion de la campana es obligatoria.");
        }

        if (metaCOP <= 0)
        {
            throw new ReglaNegocioException("La meta debe ser mayor a cero.");
        }

        if (fechaFin < fechaInicio)
        {
            throw new ReglaNegocioException("La fecha de fin no puede ser anterior a la de inicio.");
        }

        Nombre = nombre.Trim();
        Descripcion = descripcion.Trim();
        MetaCOP = metaCOP;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        EstaActiva = true;
    }

    public void ActualizarDatos(string nombre, string descripcion, decimal metaCOP, DateOnly fechaInicio, DateOnly fechaFin)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ReglaNegocioException("El nombre de la campana es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(descripcion))
        {
            throw new ReglaNegocioException("La descripcion de la campana es obligatoria.");
        }

        if (metaCOP <= 0)
        {
            throw new ReglaNegocioException("La meta debe ser mayor a cero.");
        }

        if (fechaFin < fechaInicio)
        {
            throw new ReglaNegocioException("La fecha de fin no puede ser anterior a la de inicio.");
        }

        Nombre = nombre.Trim();
        Descripcion = descripcion.Trim();
        MetaCOP = metaCOP;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
    }

    public void Cerrar() => EstaActiva = false;

    public void Reabrir() => EstaActiva = true;

    /// <summary>
    /// Si la campana admite una donacion en la fecha dada. Se comprueba contra
    /// la fecha de la donacion y no contra hoy: registrar tarde una donacion
    /// que si ocurrio dentro de la vigencia es normal.
    /// </summary>
    public bool AdmiteDonacionEn(DateOnly fecha)
    {
        return EstaActiva && fecha >= FechaInicio && fecha <= FechaFin;
    }
}
