using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class ObservationConfiguration : IEntityTypeConfiguration<Observation>
{
    public void Configure(EntityTypeBuilder<Observation> b)
    {
        b.ToTable("Observations");

        b.HasKey(o => o.Id);

        b.Property(o => o.Description)
            .IsRequired()
            .HasMaxLength(500);

        b.HasOne(o => o.Endoscopy)
            .WithMany(e => e.Observations)
            .HasForeignKey(o => o.EndoscopyId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(o => o.Finding)
            .WithMany(f => f.Observations)
            .HasForeignKey(o => o.FindingId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(o => o.OrganArea)
            .WithMany(oa => oa.Observations)
            .HasForeignKey(o => o.OrganAreaId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(o => o.EndoscopyMedia)
            .WithMany()
            .HasForeignKey(o => o.EndoscopyMediaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
