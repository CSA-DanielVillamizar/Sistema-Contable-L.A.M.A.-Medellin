using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class EventoConfiguration : IEntityTypeConfiguration<Evento>
{
    public void Configure(EntityTypeBuilder<Evento> builder)
    {
        builder.ToTable("Eventos");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nombre)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(e => e.Descripcion)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(e => e.FechaProgramada)
            .IsRequired();

        builder.Property(e => e.LugarEncuentro)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Destino)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(e => e.CuotaLogisticaCOP)
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.Property(e => e.TipoEvento)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.Estado)
            .HasConversion<int>()
            .HasDefaultValue(EstadoEvento.Programado)
            .IsRequired();

        builder.HasMany(e => e.Asistencias)
            .WithOne(a => a.Evento)
            .HasForeignKey(a => a.EventoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(e => e.Asistencias)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
