using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

/// <summary>
/// Solicitud de anulacion de un comprobante (historia 1-8).
///
/// La anulacion no es un borrado: es un hecho que alguien pide, otro aprueba y
/// queda registrado con su motivo. Sin este rastro, deshacer un movimiento
/// dependia de que alguien recordara por que se hizo.
///
/// Dos reglas la gobiernan, ambas del backlog: solo dentro del mismo mes
/// contable, y solo con aprobacion del Tesorero. Cerrado el periodo no se
/// anula nada; lo que corresponde es un ajuste contable.
/// </summary>
public sealed class SolicitudAnulacion : BaseEntity
{
    public Guid ComprobanteId { get; private set; }

    public string MotivoSolicitud { get; private set; }

    public EstadoSolicitudAnulacion Estado { get; private set; }

    public string? MotivoResolucion { get; private set; }

    /// <summary>Quien resolvio. El solicitante lo aporta CreatedBy de BaseEntity.</summary>
    public string? ResueltaPor { get; private set; }

    public DateTime? FechaResolucion { get; private set; }

    public Comprobante? Comprobante { get; private set; }

#pragma warning disable CS8618
    private SolicitudAnulacion() { }
#pragma warning restore CS8618

    public SolicitudAnulacion(Guid comprobanteId, string motivoSolicitud)
    {
        if (comprobanteId == Guid.Empty)
        {
            throw new ReglaNegocioException("ComprobanteId es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(motivoSolicitud))
        {
            throw new ReglaNegocioException("El motivo de la solicitud es obligatorio.");
        }

        ComprobanteId = comprobanteId;
        MotivoSolicitud = motivoSolicitud.Trim();
        Estado = EstadoSolicitudAnulacion.Pendiente;
    }

    public void Aprobar(string resueltaPor, string? motivoResolucion)
    {
        ValidarPendiente();

        Estado = EstadoSolicitudAnulacion.Aprobada;
        Resolver(resueltaPor, motivoResolucion);
    }

    public void Rechazar(string resueltaPor, string motivoResolucion)
    {
        ValidarPendiente();

        // Un rechazo sin motivo deja al solicitante sin saber que corregir.
        if (string.IsNullOrWhiteSpace(motivoResolucion))
        {
            throw new ReglaNegocioException("Rechazar una solicitud exige indicar el motivo.");
        }

        Estado = EstadoSolicitudAnulacion.Rechazada;
        Resolver(resueltaPor, motivoResolucion);
    }

    private void ValidarPendiente()
    {
        if (Estado != EstadoSolicitudAnulacion.Pendiente)
        {
            throw new ReglaNegocioException("La solicitud ya fue resuelta.");
        }
    }

    private void Resolver(string resueltaPor, string? motivoResolucion)
    {
        if (string.IsNullOrWhiteSpace(resueltaPor))
        {
            throw new ReglaNegocioException("No se pudo determinar quien resuelve la solicitud.");
        }

        ResueltaPor = resueltaPor.Trim();
        MotivoResolucion = string.IsNullOrWhiteSpace(motivoResolucion) ? null : motivoResolucion.Trim();
        FechaResolucion = DateTime.UtcNow;
    }
}
