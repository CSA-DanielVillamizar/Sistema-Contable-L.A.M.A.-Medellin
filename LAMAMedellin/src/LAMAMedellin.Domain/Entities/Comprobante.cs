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

    public void AgregarAsiento(AsientoContable asiento)
    {
        if (asiento is null)
        {
            throw new ArgumentNullException(nameof(asiento));
        }

        AsientosContables.Add(asiento);
    }
}
