using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class MiembroConfiguration : IEntityTypeConfiguration<Miembro>
{
    public void Configure(EntityTypeBuilder<Miembro> builder)
    {
        builder.ToTable("Miembros");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.DocumentoIdentidad)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(m => m.DocumentoIdentidad)
            .IsUnique();

        builder.Property(m => m.Nombres)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(m => m.Apellidos)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(m => m.Apodo)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.FechaIngreso)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(m => m.Rango)
            .HasConversion<int>()
            .HasDefaultValue(RangoClub.Aspirante)
            .IsRequired();

        builder.Property(m => m.TipoAfiliacion)
            .HasConversion<int>()
            .HasDefaultValue(TipoAfiliacion.Prospect)
            .IsRequired();

        builder.Property(m => m.EsActivo)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(m => m.TipoSangre)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(m => m.NombreContactoEmergencia)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(m => m.TelefonoContactoEmergencia)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(m => m.MarcaMoto)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.ModeloMoto)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Cilindraje)
            .IsRequired();

        builder.Property(m => m.Placa)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}
