using MediatR;

namespace LAMAMedellin.Application.Features.Recibos.Queries.VerificarRecibo;

/// <summary>
/// Verificacion publica de un recibo (historia 1-7). Se consulta por el
/// consecutivo, que es lo que el QR lleva codificado.
/// </summary>
public sealed record VerificarReciboQuery(string NumeroConsecutivo) : IRequest<ReciboVerificadoDto?>;

/// <summary>
/// Lo minimo para confirmar que el recibo es autentico.
///
/// No lleva tercero ni centro de costo ni descripcion a proposito: el criterio
/// de la historia exige que la verificacion publica no exponga datos
/// sensibles. Quien escanea comprueba que el movimiento existe, su fecha y su
/// valor; para lo demas hay que entrar al sistema.
/// </summary>
public sealed record ReciboVerificadoDto(
    string NumeroConsecutivo,
    DateTime Fecha,
    decimal ValorCOP,
    string Estado,
    bool EsValido);
