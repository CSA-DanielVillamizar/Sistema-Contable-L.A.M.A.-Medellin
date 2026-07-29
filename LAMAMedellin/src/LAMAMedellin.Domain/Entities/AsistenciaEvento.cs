using LAMAMedellin.Domain.Common;

namespace LAMAMedellin.Domain.Entities;

public sealed class AsistenciaEvento : BaseEntity
{
    public Guid EventoId { get; private set; }
    public Guid MiembroId { get; private set; }
    public bool Asistio { get; private set; } = false;
    public string? Observaciones { get; private set; }

    /// <summary>
    /// Snapshot de la cuota logística en COP que aplica a este miembro por el evento.
    /// Se captura en el momento del registro para conservar el valor histórico.
    /// Nulo si el evento no tiene cuota logística.
    /// </summary>
    public decimal? CuotaAplicadaCOP { get; private set; }

    public Evento? Evento { get; private set; }
    public Miembro? Miembro { get; private set; }

#pragma warning disable CS8618
    private AsistenciaEvento() { } // EF Core
#pragma warning restore CS8618

    public AsistenciaEvento(Guid eventoId, Guid miembroId, string? observaciones = null, decimal? cuotaAplicadaCop = null)
    {
        if (eventoId == Guid.Empty)
        {
            throw new ArgumentException("EventoId es obligatorio.", nameof(eventoId));
        }

        if (miembroId == Guid.Empty)
        {
            throw new ArgumentException("MiembroId es obligatorio.", nameof(miembroId));
        }

        if (cuotaAplicadaCop.HasValue && cuotaAplicadaCop.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cuotaAplicadaCop), "CuotaAplicadaCOP debe ser mayor a cero.");
        }

        EventoId = eventoId;
        MiembroId = miembroId;
        Asistio = false;
        Observaciones = ValidarObservaciones(observaciones);
        CuotaAplicadaCOP = cuotaAplicadaCop;
    }

    public void MarcarAsistencia(string? observaciones = null)
    {
        Asistio = true;
        Observaciones = ValidarObservaciones(observaciones);
    }

    public void MarcarInasistencia(string? observaciones = null)
    {
        Asistio = false;
        Observaciones = ValidarObservaciones(observaciones);
    }

    private static string? ValidarObservaciones(string? observaciones)
    {
        if (string.IsNullOrWhiteSpace(observaciones))
        {
            return null;
        }

        var limpio = observaciones.Trim();
        if (limpio.Length > 500)
        {
            throw new ArgumentException("Observaciones no puede exceder 500 caracteres.");
        }

        return limpio;
    }
}
