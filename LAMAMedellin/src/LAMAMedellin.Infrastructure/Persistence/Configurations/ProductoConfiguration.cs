using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad Producto.
/// </summary>
public sealed class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.SKU)
            .IsUnique()
            .HasDatabaseName("IX_Producto_SKU_Unique");

        builder.Property(p => p.PrecioVentaCOP)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.CantidadStock)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.CuentaContableIngresoId)
            .IsRequired();

        builder.HasOne(p => p.CuentaContableIngreso)
            .WithMany()
            .HasForeignKey(p => p.CuentaContableIngresoId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Producto_CuentaContable_Ingreso");

        builder.HasMany(p => p.Movimientos)
            .WithOne(m => m.Producto)
            .HasForeignKey(m => m.ProductoId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_MovimientoInventario_Producto");

        builder.Property(p => p.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.ToTable("Productos");
    }
}
