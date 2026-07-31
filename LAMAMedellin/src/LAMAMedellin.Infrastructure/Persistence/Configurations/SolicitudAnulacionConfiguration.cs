using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class SolicitudAnulacionConfiguration : IEntityTypeConfiguration<SolicitudAnulacion>
{
    public void Configure(EntityTypeBuilder<SolicitudAnulacion> builder)
    {
        builder.ToTable("SolicitudesAnulacion");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.MotivoSolicitud)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(s => s.MotivoResolucion)
            .HasMaxLength(500);

        builder.Property(s => s.ResueltaPor)
            .HasMaxLength(200);

        builder.Property(s => s.Estado)
            .IsRequired();

        builder.HasOne(s => s.Comprobante)
            .WithMany()
            .HasForeignKey(s => s.ComprobanteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.ComprobanteId);

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
