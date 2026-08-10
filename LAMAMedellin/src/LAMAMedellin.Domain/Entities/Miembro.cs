using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

public sealed class Miembro : BaseEntity
{
    public string DocumentoIdentidad { get; private set; }
    public string Nombres { get; private set; }
    public string Apellidos { get; private set; }
    public string Apodo { get; private set; }

    public DateOnly FechaIngreso { get; private set; }
    public RangoClub Rango { get; private set; }

    /// <summary>
    /// Determina si paga cuota mensual y a que centro de costo se imputa
    /// (historia 0-7). Antes se deducia del rango con un switch fijo que
    /// mandaba todo lo desconocido a Prospect.
    /// </summary>
    public TipoAfiliacion TipoAfiliacion { get; private set; }
    public bool EsActivo { get; private set; } = true;

    public GrupoSanguineo? TipoSangre { get; private set; }
    public string? NombreContactoEmergencia { get; private set; }
    public string? TelefonoContactoEmergencia { get; private set; }

    public string? MarcaMoto { get; private set; }
    public string? ModeloMoto { get; private set; }
    public int? Cilindraje { get; private set; }
    public string? Placa { get; private set; }

#pragma warning disable CS8618
    private Miembro() { }
#pragma warning restore CS8618

    public Miembro(
        string documentoIdentidad,
        string nombres,
        string apellidos,
        string apodo,
        DateOnly fechaIngreso,
        GrupoSanguineo? tipoSangre,
        string? nombreContactoEmergencia,
        string? telefonoContactoEmergencia,
        string? marcaMoto,
        string? modeloMoto,
        int? cilindraje,
        string? placa,
        RangoClub rango = RangoClub.Aspirante,
        TipoAfiliacion tipoAfiliacion = TipoAfiliacion.Prospect)
    {
        DocumentoIdentidad = ValidarTextoRequerido(documentoIdentidad, nameof(documentoIdentidad), 50);
        Nombres = ValidarTextoRequerido(nombres, nameof(nombres), 150);
        Apellidos = ValidarTextoRequerido(apellidos, nameof(apellidos), 150);
        Apodo = ValidarTextoOpcional(apodo, 100);

        if (fechaIngreso == default)
        {
            throw new ArgumentException("FechaIngreso es obligatoria.", nameof(fechaIngreso));
        }

        FechaIngreso = fechaIngreso;
        EsActivo = true;
        Rango = rango;
        TipoAfiliacion = tipoAfiliacion;

        // Sangre, contacto de emergencia y moto no siempre se conocen al
        // ingreso (esposas, hijos y algunos socios historicos no los tienen
        // registrados). Se guardan si vienen, sin inventar un valor cuando no.
        TipoSangre = tipoSangre;
        NombreContactoEmergencia = string.IsNullOrWhiteSpace(nombreContactoEmergencia)
            ? null
            : ValidarTextoRequerido(nombreContactoEmergencia, nameof(nombreContactoEmergencia), 150);
        TelefonoContactoEmergencia = string.IsNullOrWhiteSpace(telefonoContactoEmergencia)
            ? null
            : ValidarTelefono(telefonoContactoEmergencia, nameof(telefonoContactoEmergencia));

        MarcaMoto = string.IsNullOrWhiteSpace(marcaMoto) ? null : ValidarTextoRequerido(marcaMoto, nameof(marcaMoto), 100);
        ModeloMoto = string.IsNullOrWhiteSpace(modeloMoto) ? null : ValidarTextoRequerido(modeloMoto, nameof(modeloMoto), 100);
        Cilindraje = cilindraje is > 0 ? cilindraje : null;
        Placa = string.IsNullOrWhiteSpace(placa) ? null : ValidarPlaca(placa, nameof(placa));
    }

    public void PromoverRango(RangoClub nuevoRango)
    {
        if (!EsActivo)
        {
            throw new InvalidOperationException("No se puede promover un miembro inactivo.");
        }

        if ((int)nuevoRango < (int)Rango)
        {
            throw new InvalidOperationException("No se permite degradar rango con PromoverRango.");
        }

        Rango = nuevoRango;
    }

    public void ActualizarMotocicleta(string marcaMoto, string modeloMoto, int cilindraje, string placa)
    {
        MarcaMoto = ValidarTextoRequerido(marcaMoto, nameof(marcaMoto), 100);
        ModeloMoto = ValidarTextoRequerido(modeloMoto, nameof(modeloMoto), 100);

        if (cilindraje <= 0)
        {
            throw new ArgumentException("Cilindraje debe ser mayor que cero.", nameof(cilindraje));
        }

        Cilindraje = cilindraje;
        Placa = ValidarPlaca(placa, nameof(placa));
    }

    public void ActualizarDatosEmergencia(GrupoSanguineo tipoSangre, string nombreContactoEmergencia, string telefonoContactoEmergencia)
    {
        TipoSangre = tipoSangre;
        NombreContactoEmergencia = ValidarTextoRequerido(nombreContactoEmergencia, nameof(nombreContactoEmergencia), 150);
        TelefonoContactoEmergencia = ValidarTelefono(telefonoContactoEmergencia, nameof(telefonoContactoEmergencia));
    }

    public void CambiarTipoAfiliacion(TipoAfiliacion tipoAfiliacion)
    {
        TipoAfiliacion = tipoAfiliacion;
    }

    public void DarDeBaja()
    {
        EsActivo = false;
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

    private static string ValidarTextoOpcional(string valor, int maxLength)
    {
        var limpio = (valor ?? string.Empty).Trim();
        if (limpio.Length > maxLength)
        {
            throw new ArgumentException($"El valor no puede exceder {maxLength} caracteres.");
        }

        return limpio;
    }

    private static string ValidarTelefono(string valor, string nombreParametro)
    {
        var limpio = ValidarTextoRequerido(valor, nombreParametro, 30);

        var digitos = new string(limpio.Where(char.IsDigit).ToArray());
        if (digitos.Length < 7)
        {
            throw new ArgumentException($"{nombreParametro} no tiene un formato valido.", nombreParametro);
        }

        return limpio;
    }

    private static string ValidarPlaca(string valor, string nombreParametro)
    {
        var limpio = ValidarTextoRequerido(valor, nombreParametro, 20).ToUpperInvariant();
        return limpio;
    }
}
