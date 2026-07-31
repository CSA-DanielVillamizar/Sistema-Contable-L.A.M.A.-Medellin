using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad MovimientoInventario.
/// </summary>
public sealed class MovimientoInventarioConfiguration : IEntityTypeConfiguration<MovimientoInventario>
{
    public void Configure(EntityTypeBuilder<MovimientoInventario> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.ProductoId)
            .IsRequired();

        builder.Property(m => m.Cantidad)
            .IsRequired();

        builder.Property(m => m.TipoMovimiento)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(m => m.Fecha)
            .IsRequired();

        builder.Property(m => m.Concepto)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(m => m.Fecha)
            .HasDatabaseName("IX_MovimientoInventario_Fecha");

        builder.Property(m => m.Observaciones)
            .HasMaxLength(500);

        builder.HasOne(m => m.Producto)
            .WithMany(p => p.Movimientos)
            .HasForeignKey(m => m.ProductoId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_MovimientoInventario_Producto");

        builder.Property(m => m.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Nulo en las salidas: el costo de lo vendido se deriva del promedio
        // de las entradas, no se repite en cada movimiento.
        builder.Property(m => m.CostoUnitario)
            .HasColumnType("decimal(18,2)");

        builder.HasQueryFilter(m => !m.IsDeleted);

        builder.ToTable("MovimientosInventario");
    }
}
