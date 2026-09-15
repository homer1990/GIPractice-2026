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

        b.Property(a => a.StartDateTimeUtc).IsRequired();
        b.Property(a => a.Status).IsRequired();
        b.Property(a => a.Notes).HasMaxLength(500);
        b.Property(a => a.Urgent).HasDefaultValue(false);

        b.HasIndex(a => new { a.StartDateTimeUtc, a.Status });

        b.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(a => a.PreparationProtocol)
            .WithMany()
            .HasForeignKey(a => a.PreparationProtocolId)
            .OnDelete(DeleteBehavior.SetNull);

        // Appointment <-> Encounter is configured by EncounterConfiguration because
        // the nullable foreign key belongs to Encounter.
    }
}
