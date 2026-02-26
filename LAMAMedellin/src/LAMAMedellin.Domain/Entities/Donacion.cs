using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Domain.Entities;

public sealed class Donacion : BaseEntity
{
    public Guid DonanteId { get; private set; }
    public decimal MontoCOP { get; private set; }
    public DateTime Fecha { get; private set; }
    public Guid BancoId { get; private set; }
    public Guid CentroCostoId { get; private set; }
    public bool CertificadoEmitido { get; private set; }
    public string CodigoVerificacion { get; private set; }
    public FormaDonacion FormaDonacion { get; private set; }
    public string MedioPagoODescripcion { get; private set; }

    public Donante? Donante { get; private set; }
    public Banco? Banco { get; private set; }
    public CentroCosto? CentroCosto { get; private set; }

#pragma warning disable CS8618
    private Donacion() { }
#pragma warning restore CS8618

    public Donacion(
        Guid donanteId,
        decimal montoCop,
        DateTime fecha,
        Guid bancoId,
        Guid centroCostoId,
        string codigoVerificacion,
        FormaDonacion formaDonacion,
        string medioPagoODescripcion)
    {
        if (donanteId == Guid.Empty)
        {
            throw new ArgumentException("DonanteId requerido.", nameof(donanteId));
        }

        if (montoCop <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(montoCop), "MontoCOP debe ser mayor a cero.");
        }

        if (bancoId == Guid.Empty)
        {
            throw new ArgumentException("BancoId requerido.", nameof(bancoId));
        }

        if (centroCostoId == Guid.Empty)
        {
            throw new ArgumentException("CentroCostoId requerido.", nameof(centroCostoId));
        }

        if (string.IsNullOrWhiteSpace(codigoVerificacion))
        {
            throw new ArgumentException("CodigoVerificacion requerido.", nameof(codigoVerificacion));
        }

        if (string.IsNullOrWhiteSpace(medioPagoODescripcion))
        {
            throw new ArgumentException("MedioPagoODescripcion requerido.", nameof(medioPagoODescripcion));
        }

        DonanteId = donanteId;
        MontoCOP = montoCop;
        Fecha = fecha;
        BancoId = bancoId;
        CentroCostoId = centroCostoId;
        CertificadoEmitido = false;
        CodigoVerificacion = codigoVerificacion.Trim();
        FormaDonacion = formaDonacion;
        MedioPagoODescripcion = medioPagoODescripcion.Trim();
    }

    public void MarcarCertificadoEmitido()
    {
        CertificadoEmitido = true;
    }
}
