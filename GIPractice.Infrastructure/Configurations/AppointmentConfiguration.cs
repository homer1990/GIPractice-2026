using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> b)
    {
        b.ToTable("Appointments");

        b.HasKey(a => a.Id);

        b.Property(a => a.StartDateTimeUtc)
            .IsRequired();

        b.Property(a => a.EndDateTimeUtc);

        b.Property(a => a.Notes)
            .HasMaxLength(150);

        b.Property(a => a.Urgent)
            .HasDefaultValue(false);

        b.Property(a => a.Canceled)
            .HasDefaultValue(false);

        b.Property(a => a.TookPlace)
            .HasDefaultValue(false);

        // Patient relation
        b.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // PreparationProtocol relation
        b.HasOne(a => a.PreparationProtocol)
            .WithMany()
            .HasForeignKey(a => a.PreparationProtocolId)
            .OnDelete(DeleteBehavior.SetNull);

        // One-to-one Appointment <-> Visit
        b.HasOne(a => a.Visit)
            .WithOne(v => v.Appointment)
            .HasForeignKey<Visit>(v => v.AppointmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
