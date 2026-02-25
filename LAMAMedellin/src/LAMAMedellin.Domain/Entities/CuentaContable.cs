using LAMAMedellin.Domain.Common;

namespace LAMAMedellin.Domain.Entities;

public sealed class CuentaContable : BaseEntity
{
    public string Codigo { get; private set; }
    public string Descripcion { get; private set; }
    public bool PermiteMovimiento { get; private set; }
    public bool ExigeTercero { get; private set; }
    public List<AsientoContable> AsientosContables { get; private set; } = [];

#pragma warning disable CS8618
    private CuentaContable() { }
#pragma warning restore CS8618

    public CuentaContable(string codigo, string descripcion, bool permiteMovimiento, bool exigeTercero = false)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            throw new ArgumentException("Codigo es obligatorio.", nameof(codigo));
        }

        if (string.IsNullOrWhiteSpace(descripcion))
        {
            throw new ArgumentException("Descripcion es obligatoria.", nameof(descripcion));
        }

        Codigo = codigo.Trim();
        Descripcion = descripcion.Trim();
        PermiteMovimiento = permiteMovimiento;
        ExigeTercero = exigeTercero;
    }
}
