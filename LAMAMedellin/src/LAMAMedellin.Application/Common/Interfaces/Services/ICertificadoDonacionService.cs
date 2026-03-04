using LAMAMedellin.Application.Features.Donaciones.Queries.GetCertificadoDonacion;

namespace LAMAMedellin.Application.Common.Interfaces.Services;

public interface ICertificadoDonacionService
{
    byte[] GenerarPdf(CertificadoDonacionDto dto);
}
