using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class EgresoConfiguration : IEntityTypeConfiguration<Egreso>
{
    public void Configure(EntityTypeBuilder<Egreso> builder)
    {
        builder.ToTable("Egresos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Fecha)
            .IsRequired();

        builder.Property(x => x.Monto)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Concepto)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.TerceroId);

        builder.Property(x => x.CuentaContableId)
            .IsRequired();

        builder.Property(x => x.BancoId)
            .IsRequired();

        builder.Property(x => x.MedioPago)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CentroCostoId)
            .IsRequired();

        builder.Property(x => x.ComprobanteContableId);

        builder.HasOne(x => x.Banco)
            .WithMany()
            .HasForeignKey(x => x.BancoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CentroCosto)
            .WithMany()
            .HasForeignKey(x => x.CentroCostoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CuentaContable)
            .WithMany()
            .HasForeignKey(x => x.CuentaContableId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ComprobanteContable)
            .WithMany()
            .HasForeignKey(x => x.ComprobanteContableId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
