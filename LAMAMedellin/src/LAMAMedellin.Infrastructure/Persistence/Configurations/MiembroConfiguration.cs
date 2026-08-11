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

        // Nulo hasta que se conozca el documento; SQL Server permite varios
        // NULL en un indice unico, asi que no choca entre miembros pendientes.
        builder.Property(m => m.DocumentoIdentidad)
            .HasMaxLength(50);

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
            .HasColumnType("date");

        // La mayoria de los miembros no tiene cargo directivo (ver
        // Miembro.cs): nulo, no un valor por defecto que aparente ser un
        // cargo real.
        builder.Property(m => m.Rango)
            .HasConversion<int?>();

        builder.Property(m => m.TipoAfiliacion)
            .HasConversion<int>()
            .HasDefaultValue(TipoAfiliacion.Prospect)
            .IsRequired();

        builder.Property(m => m.EsActivo)
            .HasDefaultValue(true)
            .IsRequired();

        // Sangre, contacto de emergencia y moto no siempre se conocen (ver
        // Miembro.cs): sin esposas, hijos y varios socios historicos no
        // tendrian donde registrarse.
        builder.Property(m => m.TipoSangre)
            .HasConversion<int?>();

        builder.Property(m => m.NombreContactoEmergencia)
            .HasMaxLength(150);

        builder.Property(m => m.TelefonoContactoEmergencia)
            .HasMaxLength(30);

        builder.Property(m => m.MarcaMoto)
            .HasMaxLength(100);

        builder.Property(m => m.ModeloMoto)
            .HasMaxLength(100);

        builder.Property(m => m.Placa)
            .HasMaxLength(20);

        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}
