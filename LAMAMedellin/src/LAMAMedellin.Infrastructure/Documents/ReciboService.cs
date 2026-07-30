using LAMAMedellin.Application.Common.Interfaces.Services;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LAMAMedellin.Infrastructure.Documents;

/// <summary>
/// Recibo en PDF con codigo QR (historia 1-7).
///
/// El QR apunta a la URL publica de verificacion, no al recibo en si: quien lo
/// escanea comprueba contra el sistema que el movimiento existe y por cuanto
/// fue, en vez de leer lo que el propio papel afirma.
/// </summary>
public sealed class ReciboService : IReciboService
{
    public byte[] GenerarPdf(DatosRecibo datos)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var qr = GenerarQr(datos.UrlVerificacion);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("FUNDACIÓN L.A.M.A. MEDELLÍN").Bold().FontSize(16);
                        col.Item().Text("NIT: 902.007.705-8");
                        col.Item().PaddingTop(5).Text($"Recibo {datos.NumeroConsecutivo}").Bold().FontSize(13);
                    });

                    row.ConstantItem(90).Image(qr);
                });

                page.Content().PaddingVertical(20).Column(col =>
                {
                    col.Spacing(8);

                    Fila(col, "Fecha", datos.Fecha.ToString("yyyy-MM-dd"));
                    Fila(col, "Tercero", datos.Tercero);
                    Fila(col, "Concepto", datos.Concepto);
                    Fila(col, "Centro de costo", datos.CentroCosto);
                    Fila(col, "Medio de pago", datos.MedioPago);

                    col.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem().Text("Valor").Bold();
                        row.RelativeItem().AlignRight().Text(datos.ValorCOP.ToString("C0", CulturaCO)).Bold().FontSize(14);
                    });
                });

                page.Footer().Column(col =>
                {
                    col.Item().Text($"Código de verificación: {datos.CodigoVerificacion}").FontSize(9);
                    col.Item().Text(datos.UrlVerificacion).FontSize(8).FontColor(Colors.Grey.Darken1);
                    col.Item().PaddingTop(4)
                        .Text("Escanee el código QR para verificar este recibo contra el sistema.")
                        .FontSize(8).FontColor(Colors.Grey.Darken1);
                });
            });
        }).GeneratePdf();
    }

    private static readonly System.Globalization.CultureInfo CulturaCO = new("es-CO");

    private static void Fila(ColumnDescriptor col, string etiqueta, string valor)
    {
        col.Item().Row(row =>
        {
            row.ConstantItem(140).Text(etiqueta).SemiBold();
            row.RelativeItem().Text(valor);
        });
    }

    private static byte[] GenerarQr(string contenido)
    {
        using var generador = new QRCodeGenerator();
        using var datos = generador.CreateQrCode(contenido, QRCodeGenerator.ECCLevel.Q);
        using var png = new PngByteQRCode(datos);

        return png.GetGraphic(20);
    }
}
