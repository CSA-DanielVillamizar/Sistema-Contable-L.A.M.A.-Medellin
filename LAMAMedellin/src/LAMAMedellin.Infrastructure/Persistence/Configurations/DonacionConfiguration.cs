using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class DonacionConfiguration : IEntityTypeConfiguration<Donacion>
{
    public void Configure(EntityTypeBuilder<Donacion> builder)
    {
        builder.ToTable("Donaciones");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MontoCOP)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Fecha)
            .IsRequired();

        builder.Property(x => x.CodigoVerificacion)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.FormaDonacion)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.MedioPagoODescripcion)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasIndex(x => x.CodigoVerificacion)
            .IsUnique();

        builder.HasOne(x => x.Donante)
            .WithMany()
            .HasForeignKey(x => x.DonanteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Banco)
            .WithMany()
            .HasForeignKey(x => x.BancoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CentroCosto)
            .WithMany()
            .HasForeignKey(x => x.CentroCostoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
