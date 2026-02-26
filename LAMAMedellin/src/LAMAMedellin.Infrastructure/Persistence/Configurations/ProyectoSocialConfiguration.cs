using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class ProyectoSocialConfiguration : IEntityTypeConfiguration<ProyectoSocial>
{
    public void Configure(EntityTypeBuilder<ProyectoSocial> builder)
    {
        builder.ToTable("ProyectosSociales");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Descripcion)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(p => p.PresupuestoEstimado)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.Estado)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.FechaInicio)
            .IsRequired();

        builder.Property(p => p.FechaFin)
            .IsRequired(false);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
