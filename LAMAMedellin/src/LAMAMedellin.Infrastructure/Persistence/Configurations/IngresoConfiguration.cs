using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class IngresoConfiguration : IEntityTypeConfiguration<Ingreso>
{
    public void Configure(EntityTypeBuilder<Ingreso> builder)
    {
        builder.ToTable("Ingresos");

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

        builder.Property(x => x.CentroCostoId)
            .IsRequired();

        builder.Property(x => x.ComprobanteContableId);

        builder.HasOne<Banco>()
            .WithMany()
            .HasForeignKey(x => x.BancoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<CentroCosto>()
            .WithMany()
            .HasForeignKey(x => x.CentroCostoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<CuentaContable>()
            .WithMany()
            .HasForeignKey(x => x.CuentaContableId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Comprobante>()
            .WithMany()
            .HasForeignKey(x => x.ComprobanteContableId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
