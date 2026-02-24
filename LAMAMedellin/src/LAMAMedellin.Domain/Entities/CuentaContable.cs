using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

/// <summary>
/// Cuenta del Catálogo de Cuentas (PUC) adaptado para la Fundación L.A.M.A. Medellín.
/// Marco normativo: NIIF para Microempresas (Grupo III), ESAL vigilada por Gobernación de Antioquia.
/// La jerarquía se determina por la longitud del código numérico:
///   Clase (1 dígito) → Grupo (2) → Cuenta (4) → Subcuenta (6) → Auxiliar (8+).
/// Solo las cuentas con <see cref="PermiteMovimiento"/> = true admiten asientos contables.
/// </summary>
public sealed class CuentaContable : BaseEntity
{
    // Constructor privado para EF Core
#pragma warning disable CS8618
    private CuentaContable() { }
#pragma warning restore CS8618

    public string Codigo { get; private set; }
    public string Descripcion { get; private set; }
    public NaturalezaCuenta Naturaleza { get; private set; }

    /// <summary>
    /// Indica si la cuenta puede recibir asientos contables directos (nodo hoja).
    /// Las Clases, Grupos y Cuentas agrupadores tienen este campo en false.
    /// </summary>
    public bool PermiteMovimiento { get; private set; }

    /// <summary>
    /// Indica si todo asiento en esta cuenta debe identificar a un tercero (NIT/Cédula).
    /// Obligatorio para el reporte de información exógena a la DIAN.
    /// </summary>
    public bool ExigeTercero { get; private set; }

    /// <summary>
    /// Referencia al nodo padre en la jerarquía. Es null para las cuentas de Clase (nivel 1).
    /// </summary>
    public Guid? CuentaPadreId { get; private set; }

    public CuentaContable? CuentaPadre { get; private set; }

    /// <summary>
    /// Nivel jerárquico calculado a partir de la longitud del código numérico.
    /// </summary>
    public NivelCuenta Nivel => ResolverNivel(Codigo);

    public CuentaContable(
        string codigo,
        string descripcion,
        NaturalezaCuenta naturaleza,
        bool permiteMovimiento,
        bool exigeTercero)
    {
        Codigo = ValidarCodigo(codigo);
        Descripcion = ValidarDescripcion(descripcion);
        Naturaleza = naturaleza;
        PermiteMovimiento = permiteMovimiento;
        ExigeTercero = exigeTercero;
    }

    /// <summary>
    /// Establece el vínculo jerárquico con la cuenta padre.
    /// Solo se puede llamar durante el proceso de carga del catálogo.
    /// </summary>
    public void EstablecerPadre(Guid cuentaPadreId)
    {
        if (cuentaPadreId == Guid.Empty)
        {
            throw new ArgumentException("CuentaPadreId no puede ser Guid vacío.", nameof(cuentaPadreId));
        }

        CuentaPadreId = cuentaPadreId;
    }

    public void ActualizarDescripcion(string descripcion)
    {
        Descripcion = ValidarDescripcion(descripcion);
    }

    /// <summary>
    /// Resuelve el código del padre directo a partir del código de la cuenta.
    /// Retorna null para las cuentas de Clase (nivel 1).
    /// </summary>
    public static string? ResolverCodigoPadre(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            return null;
        }

        return codigo.Length switch
        {
            1 => null,       // Clase → sin padre
            2 => codigo[..1], // Grupo → Clase
            4 => codigo[..2], // Cuenta → Grupo
            6 => codigo[..4], // Subcuenta → Cuenta
            _ when codigo.Length >= 8 => codigo[..6], // Auxiliar → Subcuenta
            _ => null
        };
    }

    /// <summary>
    /// Determina el nivel jerárquico según la longitud del código numérico.
    /// </summary>
    public static NivelCuenta ResolverNivel(string codigo)
    {
        return codigo.Length switch
        {
            1 => NivelCuenta.Clase,
            2 => NivelCuenta.Grupo,
            4 => NivelCuenta.Cuenta,
            6 => NivelCuenta.Subcuenta,
            _ => NivelCuenta.Auxiliar
        };
    }

    // ─── Validaciones internas ───────────────────────────────────────────────

    private static string ValidarCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            throw new ArgumentException("El código de la cuenta es obligatorio.", nameof(codigo));
        }

        var c = codigo.Trim();

        if (!c.All(char.IsDigit))
        {
            throw new ArgumentException("El código de la cuenta debe contener solo dígitos numéricos.", nameof(codigo));
        }

        if (c.Length != 1 && c.Length != 2 && c.Length != 4 && c.Length != 6 && c.Length < 8)
        {
            throw new ArgumentException(
                "La longitud del código debe ser 1 (Clase), 2 (Grupo), 4 (Cuenta), 6 (Subcuenta) u 8+ (Auxiliar).",
                nameof(codigo));
        }

        return c;
    }

    private static string ValidarDescripcion(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
        {
            throw new ArgumentException("La descripción de la cuenta es obligatoria.", nameof(descripcion));
        }

        return descripcion.Trim();
    }
}
