using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class DonanteConfiguration : IEntityTypeConfiguration<Donante>
{
    public void Configure(EntityTypeBuilder<Donante> builder)
    {
        builder.ToTable("Donantes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.NombreORazonSocial)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.TipoDocumento)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.NumeroDocumento)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.TipoPersona)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => new { x.TipoDocumento, x.NumeroDocumento })
            .IsUnique();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
