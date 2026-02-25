using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

public sealed class Donante : BaseEntity
{
    public string NombreORazonSocial { get; private set; }
    public TipoDocumentoDonante TipoDocumento { get; private set; }
    public string NumeroDocumento { get; private set; }
    public string Email { get; private set; }
    public TipoPersonaDonante TipoPersona { get; private set; }

#pragma warning disable CS8618
    private Donante() { }
#pragma warning restore CS8618

    public Donante(
        string nombreORazonSocial,
        TipoDocumentoDonante tipoDocumento,
        string numeroDocumento,
        string email,
        TipoPersonaDonante tipoPersona)
    {
        NombreORazonSocial = ValidarRequerido(nombreORazonSocial, nameof(nombreORazonSocial), 200);
        TipoDocumento = tipoDocumento;
        NumeroDocumento = ValidarRequerido(numeroDocumento, nameof(numeroDocumento), 30);
        Email = ValidarRequerido(email, nameof(email), 200);
        TipoPersona = tipoPersona;
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
