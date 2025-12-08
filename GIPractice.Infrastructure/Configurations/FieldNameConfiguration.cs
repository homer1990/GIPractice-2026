using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class FieldNameConfiguration : IEntityTypeConfiguration<FieldName>
{
    public void Configure(EntityTypeBuilder<FieldName> b)
    {
        b.ToTable("FieldNames");

        b.Property(f => f.TableName)
            .HasMaxLength(128)
            .IsRequired();

        b.Property(f => f.Field)
            .HasMaxLength(128)
            .IsRequired();

        b.Property(f => f.DefaultText)
            .HasMaxLength(256)
            .IsRequired();

        b.HasIndex(f => new { f.TableName, f.Field })
            .IsUnique();

        b.HasMany(f => f.Localizations)
            .WithOne(l => l.FieldName!)
            .HasForeignKey(l => l.FieldNameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
