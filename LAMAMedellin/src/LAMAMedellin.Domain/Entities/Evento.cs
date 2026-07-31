using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

public sealed class Evento : BaseEntity
{
    private readonly List<AsistenciaEvento> _asistencias = new();

    public string Nombre { get; private set; }
    public string Descripcion { get; private set; }
    public DateTime FechaProgramada { get; private set; } // UTC
    public string LugarEncuentro { get; private set; }
    public string? Destino { get; private set; }
    public TipoEvento TipoEvento { get; private set; }
    public EstadoEvento Estado { get; private set; }

    /// <summary>
    /// Cuota logística en COP que cada miembro asistente debe aportar para cubrir
    /// gastos de alimentación, hospedaje, etc. en eventos fuera de la ciudad.
    /// Nulo si el evento no tiene cuota logística asociada.
    /// </summary>
    public decimal? CuotaLogisticaCOP { get; private set; }

    public IReadOnlyCollection<AsistenciaEvento> Asistencias => _asistencias.AsReadOnly();

#pragma warning disable CS8618
    private Evento() { } // EF Core
#pragma warning restore CS8618

    public Evento(
        string nombre,
        string descripcion,
        DateTime fechaProgramadaUtc,
        string lugarEncuentro,
        TipoEvento tipoEvento,
        string? destino = null,
        decimal? cuotaLogisticaCop = null)
    {
        Nombre = ValidarTextoRequerido(nombre, nameof(nombre), 150);
        Descripcion = ValidarTextoRequerido(descripcion, nameof(descripcion), 1000);
        LugarEncuentro = ValidarTextoRequerido(lugarEncuentro, nameof(lugarEncuentro), 200);
        Destino = ValidarTextoOpcional(destino, 200);

        if (fechaProgramadaUtc == default)
        {
            throw new ArgumentException("FechaProgramada es obligatoria.", nameof(fechaProgramadaUtc));
        }

        if (fechaProgramadaUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("FechaProgramada debe estar en UTC.", nameof(fechaProgramadaUtc));
        }

        if (cuotaLogisticaCop.HasValue && cuotaLogisticaCop.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cuotaLogisticaCop), "CuotaLogisticaCOP debe ser mayor a cero.");
        }

        FechaProgramada = fechaProgramadaUtc;
        TipoEvento = tipoEvento;
        Estado = EstadoEvento.Programado;
        CuotaLogisticaCOP = cuotaLogisticaCop;
    }

    /// <summary>
    /// Establece o actualiza la cuota logística del evento. Solo se puede modificar
    /// mientras el evento no haya finalizado o sido cancelado.
    /// </summary>
    public void EstablecerCuotaLogistica(decimal? cuotaCop)
    {
        if (Estado == EstadoEvento.Finalizado || Estado == EstadoEvento.Cancelado)
        {
            throw new ReglaNegocioException("No se puede modificar la cuota logística de un evento finalizado o cancelado.");
        }

        if (cuotaCop.HasValue && cuotaCop.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cuotaCop), "CuotaLogisticaCOP debe ser mayor a cero.");
        }

        CuotaLogisticaCOP = cuotaCop;
    }

    /// <summary>
    /// Corrige los datos del evento. Solo mientras siga programado: una vez
    /// iniciado, finalizado o cancelado ya hubo asistencias registradas contra
    /// el, y cambiarle la fecha o el lugar falsearia esa historia.
    /// </summary>
    public void ActualizarDatos(
        string nombre,
        string descripcion,
        DateTime fechaProgramadaUtc,
        string lugarEncuentro,
        TipoEvento tipoEvento,
        string? destino = null)
    {
        if (Estado != EstadoEvento.Programado)
        {
            throw new ReglaNegocioException(
                "Solo se pueden editar los datos de un evento que siga programado.");
        }

        if (fechaProgramadaUtc == default)
        {
            throw new ReglaNegocioException("FechaProgramada es obligatoria.");
        }

        Nombre = ValidarTextoRequerido(nombre, nameof(nombre), 150);
        Descripcion = ValidarTextoRequerido(descripcion, nameof(descripcion), 1000);
        LugarEncuentro = ValidarTextoRequerido(lugarEncuentro, nameof(lugarEncuentro), 200);
        Destino = ValidarTextoOpcional(destino, 200);
        FechaProgramada = fechaProgramadaUtc;
        TipoEvento = tipoEvento;
    }

    public void IniciarEvento(DateTime fechaInicioUtc)
    {
        if (Estado != EstadoEvento.Programado)
        {
            throw new InvalidOperationException("Solo se puede iniciar un evento programado.");
        }

        if (fechaInicioUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("La fecha de inicio debe estar en UTC.", nameof(fechaInicioUtc));
        }

        Estado = EstadoEvento.EnCurso;
    }

    public void FinalizarEvento(DateTime fechaFinUtc)
    {
        if (Estado != EstadoEvento.EnCurso)
        {
            throw new InvalidOperationException("Solo se puede finalizar un evento en curso.");
        }

        if (fechaFinUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("La fecha de finalizacion debe estar en UTC.", nameof(fechaFinUtc));
        }

        Estado = EstadoEvento.Finalizado;
    }

    public void CancelarEvento(string motivo)
    {
        if (Estado == EstadoEvento.Finalizado)
        {
            throw new InvalidOperationException("No se puede cancelar un evento finalizado.");
        }

        _ = ValidarTextoRequerido(motivo, nameof(motivo), 300);
        Estado = EstadoEvento.Cancelado;
    }

    private static string ValidarTextoRequerido(string valor, string nombreParametro, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ArgumentException($"{nombreParametro} es obligatorio.", nombreParametro);
        }

        var limpio = valor.Trim();
        if (limpio.Length > maxLength)
        {
            throw new ArgumentException($"{nombreParametro} no puede exceder {maxLength} caracteres.", nombreParametro);
        }

        return limpio;
    }

    private static string? ValidarTextoOpcional(string? valor, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        var limpio = valor.Trim();
        if (limpio.Length > maxLength)
        {
            throw new ArgumentException($"El valor no puede exceder {maxLength} caracteres.");
        }

        return limpio;
    }
}
