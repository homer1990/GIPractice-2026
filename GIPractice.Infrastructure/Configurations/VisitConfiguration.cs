using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class VisitConfiguration : IEntityTypeConfiguration<Visit>
{
    public void Configure(EntityTypeBuilder<Visit> b)
    {
        b.ToTable("Visits");
        b.HasKey(v => v.Id);
        b.HasIndex(v => v.EncounterId).IsUnique();
        b.Property(v => v.Notes).HasMaxLength(1000);
    }
}
