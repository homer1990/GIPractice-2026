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

        b.Property(v => v.DateOfVisitUtc)
            .IsRequired();

        b.Property(v => v.Notes)
            .HasMaxLength(500);

        // Visit -> Appointment (1:1) is configured from AppointmentConfiguration

        // Visit -> Patient (many visits per patient)
        b.HasOne(v => v.Patient)
            .WithMany(p => p.Visits)
            .HasForeignKey(v => v.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Visit -> Endoscopies (one visit, many endoscopies)
        b.HasMany(v => v.Endoscopies)
            .WithOne(e => e.Visit)
            .HasForeignKey(e => e.VisitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
