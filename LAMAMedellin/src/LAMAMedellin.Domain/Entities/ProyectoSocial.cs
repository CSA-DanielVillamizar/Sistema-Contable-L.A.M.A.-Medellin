using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

public sealed class ProyectoSocial : BaseEntity
{
    public string Nombre { get; private set; }
    public string Descripcion { get; private set; }
    public DateTime FechaInicio { get; private set; }
    public DateTime? FechaFin { get; private set; }
    public decimal PresupuestoEstimado { get; private set; }
    public EstadoProyectoSocial Estado { get; private set; }

#pragma warning disable CS8618
    private ProyectoSocial() { }
#pragma warning restore CS8618

    public ProyectoSocial(
        string nombre,
        string descripcion,
        DateTime fechaInicio,
        DateTime? fechaFin,
        decimal presupuestoEstimado,
        EstadoProyectoSocial estado)
    {
        Nombre = ValidarRequerido(nombre, nameof(nombre), 200);
        Descripcion = ValidarRequerido(descripcion, nameof(descripcion), 1000);

        if (presupuestoEstimado < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(presupuestoEstimado), "PresupuestoEstimado no puede ser negativo.");
        }

        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        PresupuestoEstimado = presupuestoEstimado;
        Estado = estado;
    }

    public void Actualizar(
        string nombre,
        string descripcion,
        DateTime fechaInicio,
        DateTime? fechaFin,
        decimal presupuestoEstimado,
        EstadoProyectoSocial estado)
    {
        Nombre = ValidarRequerido(nombre, nameof(nombre), 200);
        Descripcion = ValidarRequerido(descripcion, nameof(descripcion), 1000);

        if (presupuestoEstimado < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(presupuestoEstimado), "PresupuestoEstimado no puede ser negativo.");
        }

        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        PresupuestoEstimado = presupuestoEstimado;
        Estado = estado;
    }

    private static string ValidarRequerido(string value, string paramName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Campo obligatorio.", paramName);
        }

        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
        {
            throw new ArgumentException($"Máximo {maxLength} caracteres.", paramName);
        }

        return trimmed;
    }
}
