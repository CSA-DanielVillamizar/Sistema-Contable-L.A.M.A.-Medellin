using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

/// <summary>
/// Actividad de un proyecto social (historia 3-1).
///
/// Es lo que le da estructura al proyecto: sin actividades, un proyecto es un
/// nombre con un presupuesto y una fecha, y no hay forma de decir cuanto se ha
/// avanzado ni de rendir cuentas de en que se fue el dinero.
///
/// El presupuesto asignado es una referencia de planeacion. Lo realmente
/// ejecutado no se guarda aqui: sale de los asientos imputados al centro de
/// costo del proyecto, que es la unica cifra que el libro respalda.
/// </summary>
public sealed class ActividadProyecto : BaseEntity
{
    public Guid ProyectoSocialId { get; private set; }

    public string Nombre { get; private set; }

    public string Descripcion { get; private set; }

    public DateOnly FechaInicioPlanificada { get; private set; }
    public DateOnly FechaFinPlanificada { get; private set; }

    public decimal PresupuestoAsignado { get; private set; }

    public EstadoActividadProyecto Estado { get; private set; }

    /// <summary>Quien responde por la actividad. Texto libre: el club no tiene un maestro de responsables.</summary>
    public string? Responsable { get; private set; }

    public ProyectoSocial? ProyectoSocial { get; private set; }

#pragma warning disable CS8618
    private ActividadProyecto() { }
#pragma warning restore CS8618

    public ActividadProyecto(
        Guid proyectoSocialId,
        string nombre,
        string descripcion,
        DateOnly fechaInicioPlanificada,
        DateOnly fechaFinPlanificada,
        decimal presupuestoAsignado,
        string? responsable = null)
    {
        if (proyectoSocialId == Guid.Empty)
        {
            throw new ReglaNegocioException("ProyectoSocialId es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ReglaNegocioException("El nombre de la actividad es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(descripcion))
        {
            throw new ReglaNegocioException("La descripcion de la actividad es obligatoria.");
        }

        if (fechaFinPlanificada < fechaInicioPlanificada)
        {
            throw new ReglaNegocioException("La fecha de fin no puede ser anterior a la de inicio.");
        }

        if (presupuestoAsignado < 0)
        {
            throw new ReglaNegocioException("El presupuesto asignado no puede ser negativo.");
        }

        ProyectoSocialId = proyectoSocialId;
        Nombre = nombre.Trim();
        Descripcion = descripcion.Trim();
        FechaInicioPlanificada = fechaInicioPlanificada;
        FechaFinPlanificada = fechaFinPlanificada;
        PresupuestoAsignado = presupuestoAsignado;
        Estado = EstadoActividadProyecto.Planificada;
        Responsable = string.IsNullOrWhiteSpace(responsable) ? null : responsable.Trim();
    }

    public void ActualizarDatos(
        string nombre,
        string descripcion,
        DateOnly fechaInicioPlanificada,
        DateOnly fechaFinPlanificada,
        decimal presupuestoAsignado,
        string? responsable)
    {
        if (Estado == EstadoActividadProyecto.Completada)
        {
            throw new ReglaNegocioException(
                "Una actividad completada no se edita: cambiarle el alcance despues falsearia la rendicion.");
        }

        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ReglaNegocioException("El nombre de la actividad es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(descripcion))
        {
            throw new ReglaNegocioException("La descripcion de la actividad es obligatoria.");
        }

        if (fechaFinPlanificada < fechaInicioPlanificada)
        {
            throw new ReglaNegocioException("La fecha de fin no puede ser anterior a la de inicio.");
        }

        if (presupuestoAsignado < 0)
        {
            throw new ReglaNegocioException("El presupuesto asignado no puede ser negativo.");
        }

        Nombre = nombre.Trim();
        Descripcion = descripcion.Trim();
        FechaInicioPlanificada = fechaInicioPlanificada;
        FechaFinPlanificada = fechaFinPlanificada;
        PresupuestoAsignado = presupuestoAsignado;
        Responsable = string.IsNullOrWhiteSpace(responsable) ? null : responsable.Trim();
    }

    /// <summary>
    /// Avanza el estado. Una actividad cancelada o completada ya no cambia: el
    /// estado final es parte de lo que se rinde.
    /// </summary>
    public void CambiarEstado(EstadoActividadProyecto nuevoEstado)
    {
        if (Estado is EstadoActividadProyecto.Completada or EstadoActividadProyecto.Cancelada)
        {
            throw new ReglaNegocioException($"Una actividad {Estado} ya no cambia de estado.");
        }

        Estado = nuevoEstado;
    }
}
