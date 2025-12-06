using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class VersionHistoryConfiguration : IEntityTypeConfiguration<VersionHistory>
{
    public void Configure(EntityTypeBuilder<VersionHistory> b)
    {
        b.ToTable("VersionHistory");

        b.HasKey(v => v.Id);

        b.Property(v => v.EntityName)
            .IsRequired()
            .HasMaxLength(200);

        b.Property(v => v.SnapshotJson)
            .IsRequired();

        b.Property(v => v.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        b.Property(v => v.CreatedAtUtc)
            .IsRequired();

        b.HasIndex(v => new { v.EntityName, v.EntityId, v.Version })
            .IsUnique();
    }
}
