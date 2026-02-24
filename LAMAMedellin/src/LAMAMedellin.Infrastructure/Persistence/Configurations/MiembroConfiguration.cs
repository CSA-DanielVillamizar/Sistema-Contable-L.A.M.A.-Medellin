using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class MiembroConfiguration : IEntityTypeConfiguration<Miembro>
{
    public void Configure(EntityTypeBuilder<Miembro> builder)
    {
        builder.ToTable("Miembros");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Apellidos)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(m => m.Documento)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(m => m.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(m => m.Telefono)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(m => m.TipoAfiliacion)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(m => m.Estado)
            .HasConversion<int>()
            .IsRequired();

        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}
