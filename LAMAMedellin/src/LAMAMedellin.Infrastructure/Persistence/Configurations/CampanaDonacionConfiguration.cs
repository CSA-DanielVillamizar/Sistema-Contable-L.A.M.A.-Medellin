using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class CampanaDonacionConfiguration : IEntityTypeConfiguration<CampanaDonacion>
{
    public void Configure(EntityTypeBuilder<CampanaDonacion> builder)
    {
        builder.ToTable("CampanasDonacion");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Descripcion)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(c => c.MetaCOP)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.FechaInicio)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(c => c.FechaFin)
            .HasColumnType("date")
            .IsRequired();

        builder.HasMany(c => c.Donaciones)
            .WithOne(d => d.CampanaDonacion)
            .HasForeignKey(d => d.CampanaDonacionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
