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

        builder.Property(x => x.PermiteMovimiento)
            .IsRequired();

        builder.Property(x => x.ExigeTercero)
            .IsRequired();

        builder.HasIndex(x => x.Codigo)
            .IsUnique();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
