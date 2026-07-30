using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class CuentaPorPagarConfiguration : IEntityTypeConfiguration<CuentaPorPagar>
{
    public void Configure(EntityTypeBuilder<CuentaPorPagar> builder)
    {
        builder.ToTable("CuentasPorPagar");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.NombreProveedor)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.NitProveedor)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.NumeroFactura)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Concepto)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(c => c.FechaEmision)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(c => c.FechaVencimiento)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(c => c.ValorTotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.SaldoPendiente)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.Estado)
            .IsRequired();

        // Nulos para las obligaciones en pesos, que son la mayoria.
        builder.Property(c => c.ValorUSD)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.TasaCambioReconocida)
            .HasColumnType("decimal(18,4)");

        builder.Ignore(c => c.EsEnMonedaExtranjera);

        builder.HasOne(c => c.CuentaContableGasto)
            .WithMany()
            .HasForeignKey(c => c.CuentaContableGastoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.CentroCosto)
            .WithMany()
            .HasForeignKey(c => c.CentroCostoId)
            .OnDelete(DeleteBehavior.Restrict);

        // La misma factura del mismo proveedor no puede entrar dos veces:
        // duplicaria el pasivo y el gasto. Se filtra por IsDeleted para que una
        // factura dada de baja no bloquee volver a registrarla.
        builder.HasIndex(c => new { c.NitProveedor, c.NumeroFactura })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
