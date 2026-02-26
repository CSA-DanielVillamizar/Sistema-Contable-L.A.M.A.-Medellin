using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class AsientoContableConfiguration : IEntityTypeConfiguration<AsientoContable>
{
    public void Configure(EntityTypeBuilder<AsientoContable> builder)
    {
        builder.ToTable("AsientosContables");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CuentaContableId)
            .IsRequired();

        builder.Property(x => x.TerceroId);

        builder.Property(x => x.CentroCostoId)
            .IsRequired();

        builder.Property(x => x.Debe)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Haber)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Referencia)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasOne(x => x.CuentaContable)
            .WithMany(x => x.AsientosContables)
            .HasForeignKey(x => x.CuentaContableId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CentroCosto)
            .WithMany(x => x.AsientosContables)
            .HasForeignKey(x => x.CentroCostoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasCheckConstraint("CK_AsientoContable_DebeHaber_Exclusivo", "(([Debe] > 0 AND [Haber] = 0) OR ([Debe] = 0 AND [Haber] > 0))");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
