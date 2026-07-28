using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class ConsecutivoComprobanteConfiguration : IEntityTypeConfiguration<ConsecutivoComprobante>
{
    public void Configure(EntityTypeBuilder<ConsecutivoComprobante> builder)
    {
        builder.ToTable("ConsecutivosComprobante");

        builder.HasKey(c => c.TipoComprobante);

        builder.Property(c => c.TipoComprobante)
            .HasConversion<int>()
            .ValueGeneratedNever();

        builder.Property(c => c.UltimoNumero)
            .IsRequired();
    }
}
