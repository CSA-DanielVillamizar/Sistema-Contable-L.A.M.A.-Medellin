using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Enums;
using LAMAMedellin.Domain.ValueObjects;

namespace LAMAMedellin.Domain.Entities;

public sealed class Transaccion : BaseEntity
{
    private Transaccion()
    {
    }

    public decimal MontoCOP { get; private set; }
    public DateTime Fecha { get; private set; }
    public TipoTransaccion Tipo { get; private set; }
    public MedioPago MedioPago { get; private set; }
    public Guid CentroCostoId { get; private set; }
    public Guid BancoId { get; private set; }
    public string Descripcion { get; private set; } = string.Empty;

    public CentroCosto? CentroCosto { get; private set; }
    public Banco? Banco { get; private set; }

    public TransaccionMultimoneda? TransaccionMultimoneda { get; private set; }

    public Transaccion(
        decimal montoCOP,
        DateTime fecha,
        TipoTransaccion tipo,
        MedioPago medioPago,
        Guid centroCostoId,
        Guid bancoId,
        string descripcion,
        TransaccionMultimoneda? transaccionMultimoneda = null)
    {
        if (montoCOP <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(montoCOP), "MontoCOP debe ser mayor a cero.");
        }

        if (centroCostoId == Guid.Empty)
        {
            throw new ArgumentException("CentroCostoId es obligatorio.", nameof(centroCostoId));
        }

        if (bancoId == Guid.Empty)
        {
            throw new ArgumentException("BancoId es obligatorio.", nameof(bancoId));
        }

        if (string.IsNullOrWhiteSpace(descripcion))
        {
            throw new ArgumentException("Descripcion es obligatoria.", nameof(descripcion));
        }

        MontoCOP = montoCOP;
        Fecha = fecha;
        Tipo = tipo;
        MedioPago = medioPago;
        CentroCostoId = centroCostoId;
        BancoId = bancoId;
        Descripcion = descripcion.Trim();
        TransaccionMultimoneda = transaccionMultimoneda;
    }

    public void VincularCentroCosto(CentroCosto centroCosto)
    {
        CentroCosto = centroCosto ?? throw new ArgumentNullException(nameof(centroCosto));
        CentroCostoId = centroCosto.Id;
    }

    public void VincularBanco(Banco banco)
    {
        Banco = banco ?? throw new ArgumentNullException(nameof(banco));
        BancoId = banco.Id;
    }

    public void AsignarMultimoneda(TransaccionMultimoneda transaccionMultimoneda)
    {
        TransaccionMultimoneda = transaccionMultimoneda ?? throw new ArgumentNullException(nameof(transaccionMultimoneda));
    }

    public void RemoverMultimoneda()
    {
        TransaccionMultimoneda = null;
    }
}
