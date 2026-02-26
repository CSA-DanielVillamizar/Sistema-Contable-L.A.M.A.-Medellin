using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class ArticuloConfiguration : IEntityTypeConfiguration<Articulo>
{
    public void Configure(EntityTypeBuilder<Articulo> builder)
    {
        builder.ToTable("Articulos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.SKU)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Categoria)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.PrecioVenta)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.CostoPromedio)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.StockActual)
            .IsRequired();

        builder.HasIndex(x => x.SKU)
            .IsUnique();

        builder.HasOne(x => x.CuentaContableIngreso)
            .WithMany()
            .HasForeignKey(x => x.CuentaContableIngresoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
