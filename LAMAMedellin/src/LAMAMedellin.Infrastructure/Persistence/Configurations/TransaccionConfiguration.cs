using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class TransaccionConfiguration : IEntityTypeConfiguration<Transaccion>
{
    public void Configure(EntityTypeBuilder<Transaccion> builder)
    {
        builder.ToTable("Transacciones");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.MontoCOP)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(t => t.Fecha)
            .IsRequired();

        builder.Property(t => t.Tipo)
            .IsRequired();

        builder.Property(t => t.MedioPago)
            .IsRequired();

        builder.Property(t => t.CentroCostoId)
            .IsRequired();

        builder.Property(t => t.BancoId)
            .IsRequired();

        builder.Property(t => t.Descripcion)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasOne(t => t.CentroCosto)
            .WithMany()
            .HasForeignKey(t => t.CentroCostoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Banco)
            .WithMany()
            .HasForeignKey(t => t.BancoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(t => t.TransaccionMultimoneda, owned =>
        {
            owned.Property(m => m.MonedaOrigen)
                .HasColumnName("MonedaOrigen")
                .HasMaxLength(10);

            owned.Property(m => m.MontoMonedaOrigen)
                .HasColumnName("MontoMonedaOrigen")
                .HasPrecision(18, 2);

            owned.Property(m => m.TasaCambioUsada)
                .HasColumnName("TasaCambioUsada")
                .HasPrecision(18, 2);

            owned.Property(m => m.FechaTasaCambio)
                .HasColumnName("FechaTasaCambio");

            owned.Property(m => m.Fuente)
                .HasColumnName("FuenteTasaCambio");
        });

        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
