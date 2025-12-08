using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class LocalizationConfiguration : IEntityTypeConfiguration<Localization>
{
    public void Configure(EntityTypeBuilder<Localization> b)
    {
        b.ToTable("Localizations");

        b.Property(l => l.CultureName)
            .HasMaxLength(12)
            .IsRequired();

        b.Property(l => l.Value)
            .HasMaxLength(512)
            .IsRequired();

        b.HasIndex(l => new { l.FieldNameId, l.CultureName })
            .IsUnique();
    }
}
