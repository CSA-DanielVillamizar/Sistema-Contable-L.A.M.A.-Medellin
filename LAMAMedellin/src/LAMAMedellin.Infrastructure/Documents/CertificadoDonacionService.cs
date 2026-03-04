using LAMAMedellin.Application.Common.Interfaces.Services;
using LAMAMedellin.Application.Features.Donaciones.Queries.GetCertificadoDonacion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LAMAMedellin.Infrastructure.Documents;

public sealed class CertificadoDonacionService : ICertificadoDonacionService
{
    public byte[] GenerarPdf(CertificadoDonacionDto dto)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().Text("FUNDACIÓN L.A.M.A. MEDELLÍN").Bold().FontSize(16);
                    col.Item().Text("NIT: 900.000.000-0");
                    col.Item().Text($"Certificado de Donación - Año gravable {dto.AnioGravable}");
                });

                page.Content().PaddingVertical(15).Column(col =>
                {
                    col.Spacing(8);
                    col.Item().Text($"Fecha: {dto.Fecha:yyyy-MM-dd}");
                    col.Item().Text($"Código verificación: {dto.CodigoVerificacion}");
                    col.Item().Text($"Donante: {dto.NombreDonante}");
                    col.Item().Text($"Documento: {dto.TipoDocumento} {dto.NumeroDocumento}");
                    col.Item().Text($"Monto COP: {dto.MontoCOP:N2}");
                    col.Item().Text($"Monto en letras: {dto.MontoEnLetras}");
                    col.Item().Text($"Forma de donación: {dto.FormaDonacion}");
                    col.Item().Text($"Medio de pago / detalle: {dto.MedioPagoODescripcion}");

                    col.Item().PaddingTop(12).Text("Declaraciones legales").Bold();
                    col.Item().Text("• Los recursos se destinan al objeto social de la Fundación.");
                    col.Item().Text("• Esta donación no constituye contraprestación por servicios.");
                    col.Item().Text("• Se certifica cumplimiento de Art. 125-1 y 125-2 E.T.");
                });

                page.Footer().AlignRight().Text("Generado por Sistema Contable L.A.M.A. Medellín");
            });
        }).GeneratePdf();
    }
}
