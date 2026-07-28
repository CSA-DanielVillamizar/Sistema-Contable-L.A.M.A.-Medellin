using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Services;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence;

public sealed class GeneradorConsecutivos(LamaDbContext context) : IGeneradorConsecutivos
{
    public async Task<string> SiguienteAsync(
        TipoComprobante tipoComprobante,
        CancellationToken cancellationToken = default)
    {
        var tipo = (int)tipoComprobante;

        // Incremento atomico: el UPDATE toma el bloqueo de la fila y devuelve el
        // valor ya incrementado en la misma sentencia, asi que dos peticiones
        // simultaneas no pueden obtener el mismo numero. Participa de la
        // transaccion ambiente, de modo que si la operacion se revierte, el
        // consecutivo tambien.
        var numeros = await context.Database
            .SqlQuery<int>($@"
                UPDATE ConsecutivosComprobante
                SET UltimoNumero = UltimoNumero + 1
                OUTPUT INSERTED.UltimoNumero AS Value
                WHERE TipoComprobante = {tipo}")
            .ToListAsync(cancellationToken);

        if (numeros.Count == 0)
        {
            throw new ExcepcionNegocio(
                $"No hay contador de consecutivos configurado para el tipo de comprobante {tipoComprobante}.");
        }

        return $"{Prefijo(tipoComprobante)}-{numeros[0]:D8}";
    }

    private static string Prefijo(TipoComprobante tipoComprobante) => tipoComprobante switch
    {
        TipoComprobante.Ingreso => "ING",
        TipoComprobante.Egreso => "EGR",
        TipoComprobante.Diario => "DIA",
        TipoComprobante.Ajuste => "AJU",
        TipoComprobante.Cierre => "CIE",
        _ => "CMP",
    };
}
