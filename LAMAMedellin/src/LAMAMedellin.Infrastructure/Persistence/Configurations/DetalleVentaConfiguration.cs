using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class DetalleVentaConfiguration : IEntityTypeConfiguration<DetalleVenta>
{
    public void Configure(EntityTypeBuilder<DetalleVenta> builder)
    {
        builder.ToTable("DetallesVenta");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Cantidad)
            .IsRequired();

        builder.Property(x => x.PrecioUnitario)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Subtotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne(x => x.Venta)
            .WithMany(x => x.DetallesVenta)
            .HasForeignKey(x => x.VentaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Articulo)
            .WithMany(x => x.DetallesVenta)
            .HasForeignKey(x => x.ArticuloId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
