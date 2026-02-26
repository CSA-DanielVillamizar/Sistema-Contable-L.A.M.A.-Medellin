using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class CuentaContableConfiguration : IEntityTypeConfiguration<CuentaContable>
{
    public void Configure(EntityTypeBuilder<CuentaContable> builder)
    {
        builder.ToTable("CuentasContables");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Codigo)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.Naturaleza)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.PermiteMovimiento)
            .IsRequired();

        builder.Property(x => x.ExigeTercero)
            .IsRequired();

        builder.Property(x => x.CuentaPadreId);

        builder.HasIndex(x => x.Codigo)
            .IsUnique();

        builder.HasIndex(x => x.CuentaPadreId);

        builder.HasOne(x => x.CuentaPadre)
            .WithMany(x => x.CuentasHijas)
            .HasForeignKey(x => x.CuentaPadreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
