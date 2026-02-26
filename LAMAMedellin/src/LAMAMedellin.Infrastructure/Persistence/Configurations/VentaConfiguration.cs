using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class VentaConfiguration : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.ToTable("Ventas");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.NumeroFacturaInterna)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Fecha)
            .IsRequired();

        builder.Property(x => x.Total)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.MetodoPago)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => x.NumeroFacturaInterna)
            .IsUnique();

        builder.HasMany(x => x.DetallesVenta)
            .WithOne(x => x.Venta)
            .HasForeignKey(x => x.VentaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
