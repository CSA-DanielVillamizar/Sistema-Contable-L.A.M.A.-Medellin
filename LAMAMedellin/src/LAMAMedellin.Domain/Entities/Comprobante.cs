using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

public sealed class Comprobante : BaseEntity
{
    public string NumeroConsecutivo { get; private set; }
    public DateTime Fecha { get; private set; }
    public TipoComprobante TipoComprobante { get; private set; }
    public string Descripcion { get; private set; }
    public EstadoComprobante EstadoComprobante { get; private set; }
    public List<AsientoContable> AsientosContables { get; private set; } = [];

#pragma warning disable CS8618
    private Comprobante() { }
#pragma warning restore CS8618

    public Comprobante(
        string numeroConsecutivo,
        DateTime fecha,
        TipoComprobante tipoComprobante,
        string descripcion,
        EstadoComprobante estadoComprobante)
    {
        if (string.IsNullOrWhiteSpace(numeroConsecutivo))
        {
            throw new ArgumentException("NumeroConsecutivo es obligatorio.", nameof(numeroConsecutivo));
        }

        if (string.IsNullOrWhiteSpace(descripcion))
        {
            throw new ArgumentException("Descripcion es obligatoria.", nameof(descripcion));
        }

        NumeroConsecutivo = numeroConsecutivo.Trim();
        Fecha = fecha;
        TipoComprobante = tipoComprobante;
        Descripcion = descripcion.Trim();
        EstadoComprobante = estadoComprobante;
    }

    /// <summary>
    /// Anula el comprobante (historia 1-8). Solo dentro del mismo mes contable:
    /// pasado el mes, el movimiento ya entro en los informes que alguien vio, y
    /// borrarlo hacia atras los desmiente. Lo que corresponde entonces es un
    /// ajuste contable, que deja rastro de ambas cosas.
    ///
    /// Quien decide si procede es el caso de uso; aqui solo se impone la regla
    /// del mes y la de no anular dos veces.
    /// </summary>
    public void Anular(DateTime fechaActualUtc)
    {
        if (EstadoComprobante == EstadoComprobante.Anulado)
        {
            throw new ReglaNegocioException("El comprobante ya esta anulado.");
        }

        if (Fecha.Year != fechaActualUtc.Year || Fecha.Month != fechaActualUtc.Month)
        {
            throw new ReglaNegocioException(
                "Solo se puede anular un comprobante dentro de su mismo mes contable. "
                + "Para un mes anterior corresponde un ajuste contable.");
        }

        EstadoComprobante = EstadoComprobante.Anulado;
    }

    public void AgregarAsiento(AsientoContable asiento)
    {
        if (asiento is null)
        {
            throw new ArgumentNullException(nameof(asiento));
        }

        AsientosContables.Add(asiento);
    }
}
