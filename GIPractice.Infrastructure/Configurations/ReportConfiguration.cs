using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> b)
    {
        b.ToTable("Reports");

        b.HasKey(r => r.Id);

        b.Property(r => r.IsUrgent)
            .IsRequired();

        b.Property(r => r.Summary)
            .HasMaxLength(4000);

        b.HasOne(r => r.Endoscopy)
            .WithOne(e => e.Report)
            .HasForeignKey<Report>(r => r.EndoscopyId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(r => r.File)
            .WithMany()
            .HasForeignKey(r => r.FileId)
            .OnDelete(DeleteBehavior.SetNull);

        // IMPORTANT:
        // This navigation is a *derived* view over Endoscopy.BiopsyBottles;
        // we don't want a separate join table, so we ignore it.
        b.Ignore(r => r.BiopsyBottles);
    }
}
