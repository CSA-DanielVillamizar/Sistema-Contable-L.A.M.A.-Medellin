using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Queries.GetCertificadoDonacion;

public sealed class GetCertificadoDonacionQueryHandler(IDonacionRepository donacionRepository)
    : IRequestHandler<GetCertificadoDonacionQuery, CertificadoDonacionDto?>
{
    public async Task<CertificadoDonacionDto?> Handle(GetCertificadoDonacionQuery request, CancellationToken cancellationToken)
    {
        var donacion = await donacionRepository.GetByIdWithDetallesAsync(request.DonacionId, cancellationToken);
        if (donacion is null || donacion.Donante is null)
        {
            return null;
        }

        return new CertificadoDonacionDto(
            donacion.Id,
            donacion.Fecha,
            donacion.MontoCOP,
            ConvertirMontoEnLetras(donacion.MontoCOP),
            donacion.CodigoVerificacion,
            donacion.DonanteId,
            donacion.Donante.NombreORazonSocial,
            donacion.Donante.TipoDocumento.ToString(),
            donacion.Donante.NumeroDocumento,
            donacion.Donante.Email,
            donacion.FormaDonacion,
            donacion.MedioPagoODescripcion,
            2026);
    }

    private static string ConvertirMontoEnLetras(decimal monto)
    {
        var entero = (long)Math.Truncate(monto);
        var centavos = (int)Math.Round((monto - entero) * 100, MidpointRounding.AwayFromZero);
        return $"{NumeroALetras(entero)} pesos con {centavos:00}/100 M/CTE";
    }

    private static string NumeroALetras(long numero)
    {
        if (numero == 0)
        {
            return "cero";
        }

        if (numero < 0)
        {
            return $"menos {NumeroALetras(Math.Abs(numero))}";
        }

        if (numero <= 20)
        {
            string[] unidades =
            {
                "cero", "uno", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve", "diez",
                "once", "doce", "trece", "catorce", "quince", "dieciséis", "diecisiete", "dieciocho", "diecinueve", "veinte"
            };

            return unidades[numero];
        }

        if (numero < 30)
        {
            return numero == 21 ? "veintiuno" : $"veinti{NumeroALetras(numero - 20)}";
        }

        if (numero < 100)
        {
            string[] decenas = { "", "", "", "treinta", "cuarenta", "cincuenta", "sesenta", "setenta", "ochenta", "noventa" };
            var decena = numero / 10;
            var unidad = numero % 10;
            return unidad == 0 ? decenas[decena] : $"{decenas[decena]} y {NumeroALetras(unidad)}";
        }

        if (numero == 100)
        {
            return "cien";
        }

        if (numero < 1000)
        {
            string[] centenas =
            {
                "", "ciento", "doscientos", "trescientos", "cuatrocientos", "quinientos", "seiscientos", "setecientos", "ochocientos", "novecientos"
            };

            var centena = numero / 100;
            var restante = numero % 100;
            return restante == 0 ? centenas[centena] : $"{centenas[centena]} {NumeroALetras(restante)}";
        }

        if (numero < 1_000_000)
        {
            var miles = numero / 1000;
            var restante = numero % 1000;
            var prefijo = miles == 1 ? "mil" : $"{NumeroALetras(miles)} mil";
            return restante == 0 ? prefijo : $"{prefijo} {NumeroALetras(restante)}";
        }

        if (numero < 1_000_000_000)
        {
            var millones = numero / 1_000_000;
            var restante = numero % 1_000_000;
            var prefijo = millones == 1 ? "un millón" : $"{NumeroALetras(millones)} millones";
            return restante == 0 ? prefijo : $"{prefijo} {NumeroALetras(restante)}";
        }

        return numero.ToString();
    }
}
