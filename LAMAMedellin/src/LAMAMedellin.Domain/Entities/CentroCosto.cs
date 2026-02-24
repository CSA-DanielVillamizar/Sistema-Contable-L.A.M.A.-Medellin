using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

public sealed class CentroCosto : BaseEntity
{
    public string Nombre { get; private set; }
    public TipoCentroCosto Tipo { get; private set; }

    public CentroCosto(string nombre, TipoCentroCosto tipo)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("Nombre es obligatorio.", nameof(nombre));
        }

        Nombre = nombre.Trim();
        Tipo = tipo;
    }

    public void ActualizarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("Nombre es obligatorio.", nameof(nombre));
        }

        Nombre = nombre.Trim();
    }

    public void ActualizarTipo(TipoCentroCosto tipo)
    {
        Tipo = tipo;
    }
}
