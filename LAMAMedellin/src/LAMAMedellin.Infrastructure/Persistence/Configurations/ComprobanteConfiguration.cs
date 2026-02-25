using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class ComprobanteConfiguration : IEntityTypeConfiguration<Comprobante>
{
    public void Configure(EntityTypeBuilder<Comprobante> builder)
    {
        builder.ToTable("Comprobantes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.NumeroConsecutivo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Fecha)
            .IsRequired();

        builder.Property(x => x.TipoComprobante)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.EstadoComprobante)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => x.NumeroConsecutivo)
            .IsUnique();

        builder.HasMany(x => x.AsientosContables)
            .WithOne(x => x.Comprobante)
            .HasForeignKey(x => x.ComprobanteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
