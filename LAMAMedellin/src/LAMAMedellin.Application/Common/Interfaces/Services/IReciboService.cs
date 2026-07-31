namespace LAMAMedellin.Application.Common.Interfaces.Services;

/// <summary>
/// Datos que van en el recibo (historia 1-7). Son exactamente los que enumera
/// el criterio de aceptacion, ni mas ni menos.
/// </summary>
public sealed record DatosRecibo(
    string NumeroConsecutivo,
    DateTime Fecha,
    string Tercero,
    string Concepto,
    decimal ValorCOP,
    string CentroCosto,
    string MedioPago,
    string CodigoVerificacion,
    string UrlVerificacion);

public interface IReciboService
{
    byte[] GenerarPdf(DatosRecibo datos);
}
