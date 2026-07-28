using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

/// <summary>
/// Periodo contable mensual y su estado de cierre.
///
/// Sin esta entidad ningun dato del sistema era nunca definitivo: cualquier mes
/// podia seguir modificandose indefinidamente, que es lo contrario de lo que
/// exige una contabilidad formal.
///
/// El flujo lo define la historia 1-5 del backlog: el Tesorero valida el mes y
/// el Contador lo cierra. Son dos personas distintas a proposito, por
/// segregacion de funciones.
/// </summary>
public sealed class PeriodoContable : BaseEntity
{
    public int Anio { get; private set; }
    public int Mes { get; private set; }
    public EstadoPeriodoContable Estado { get; private set; }

    // Quien valido y quien cerro son dos hechos de negocio distintos, asi que se
    // guardan aparte de CreatedBy/UpdatedBy, que solo conservan el ultimo toque.
    public DateTime? FechaValidacionTesoreria { get; private set; }
    public string? ValidadoPor { get; private set; }
    public DateTime? FechaCierre { get; private set; }
    public string? CerradoPor { get; private set; }

#pragma warning disable CS8618
    private PeriodoContable() { }
#pragma warning restore CS8618

    public PeriodoContable(int anio, int mes)
    {
        if (anio < 2000 || anio > 2999)
        {
            throw new ReglaNegocioException("El anio del periodo contable no es valido.");
        }

        if (mes is < 1 or > 12)
        {
            throw new ReglaNegocioException("El mes del periodo contable debe estar entre 1 y 12.");
        }

        Anio = anio;
        Mes = mes;
        Estado = EstadoPeriodoContable.Abierto;
    }

    /// <summary>Marca del Tesorero previa al cierre (historia 1-5).</summary>
    public void ValidarTesoreria(string? usuario)
    {
        if (Estado == EstadoPeriodoContable.Cerrado)
        {
            throw new ReglaNegocioException("El periodo ya esta cerrado y no admite validacion.");
        }

        if (Estado == EstadoPeriodoContable.ValidadoTesoreria)
        {
            throw new ReglaNegocioException("El periodo ya fue validado por tesoreria.");
        }

        Estado = EstadoPeriodoContable.ValidadoTesoreria;
        FechaValidacionTesoreria = DateTime.UtcNow;
        ValidadoPor = usuario;
    }

    /// <summary>
    /// Cierre ejecutado por el Contador. Exige la validacion previa de
    /// tesoreria: cerrar sin ella saltaria el control de segregacion.
    /// </summary>
    public void Cerrar(string? usuario)
    {
        if (Estado == EstadoPeriodoContable.Cerrado)
        {
            throw new ReglaNegocioException("El periodo ya esta cerrado.");
        }

        if (Estado != EstadoPeriodoContable.ValidadoTesoreria)
        {
            throw new ReglaNegocioException(
                "El periodo debe ser validado por tesoreria antes de cerrarse.");
        }

        Estado = EstadoPeriodoContable.Cerrado;
        FechaCierre = DateTime.UtcNow;
        CerradoPor = usuario;
    }

    public bool EstaCerrado => Estado == EstadoPeriodoContable.Cerrado;

    public bool Contiene(DateTime fecha) => fecha.Year == Anio && fecha.Month == Mes;
}
