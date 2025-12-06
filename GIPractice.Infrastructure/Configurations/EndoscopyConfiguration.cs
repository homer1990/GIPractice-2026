using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class EndoscopyConfiguration : IEntityTypeConfiguration<Endoscopy>
{
    public void Configure(EntityTypeBuilder<Endoscopy> b)
    {
        b.ToTable("Endoscopies");

        b.HasKey(e => e.Id);

        b.Property(e => e.PerformedAtUtc)
            .IsRequired();

        b.Property(e => e.Notes)
            .HasMaxLength(1000);

        b.Property(e => e.IsUrgent)
            .HasDefaultValue(false);

        b.Property(e => e.EndoscopyCost)
            .HasColumnType("decimal(18,2)");

        b.Property(e => e.BiopsiesCost)
            .HasColumnType("decimal(18,2)");

        // Endoscopy -> Patient
        b.HasOne(e => e.Patient)
            .WithMany(p => p.Endoscopies)
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Endoscopy -> Visit is configured from VisitConfiguration side

        // Endoscopy -> Report (0..1)
        b.HasOne(e => e.Report)
            .WithOne(r => r.Endoscopy)
            .HasForeignKey<Report>(r => r.EndoscopyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Endoscopy -> BiopsyBottles
        b.HasMany(e => e.BiopsyBottles)
            .WithOne(bottle => bottle.Endoscopy)
            .HasForeignKey(bottle => bottle.EndoscopyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Endoscopy -> MediaFiles
        b.HasMany(e => e.MediaFiles)
            .WithOne(m => m.Endoscopy)
            .HasForeignKey(m => m.EndoscopyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
