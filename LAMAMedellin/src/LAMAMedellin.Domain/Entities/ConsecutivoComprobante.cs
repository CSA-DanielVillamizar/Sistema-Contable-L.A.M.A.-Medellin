using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

/// <summary>
/// Contador persistido del consecutivo por tipo de comprobante.
///
/// Existe porque la numeracion consecutiva es un requisito contable: debe ser
/// secuencial, unica y verificable. Antes se derivaba de la marca de tiempo
/// (CMP-20260727143052123), lo que no es consecutivo, deja huecos arbitrarios y
/// colisiona si dos comprobantes se registran en el mismo milisegundo, contra
/// el indice unico de NumeroConsecutivo.
///
/// No hereda de BaseEntity: es una tabla de apoyo, no un hecho contable.
/// </summary>
public sealed class ConsecutivoComprobante
{
    public TipoComprobante TipoComprobante { get; private set; }
    public int UltimoNumero { get; private set; }

#pragma warning disable CS8618
    private ConsecutivoComprobante() { }
#pragma warning restore CS8618

    public ConsecutivoComprobante(TipoComprobante tipoComprobante, int ultimoNumero = 0)
    {
        if (ultimoNumero < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ultimoNumero), "UltimoNumero no puede ser negativo.");
        }

        TipoComprobante = tipoComprobante;
        UltimoNumero = ultimoNumero;
    }
}
