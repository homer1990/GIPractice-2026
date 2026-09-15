using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public sealed class EncounterConfiguration : IEntityTypeConfiguration<Encounter>
{
    public void Configure(EntityTypeBuilder<Encounter> b)
    {
        b.ToTable("Encounters");
        b.HasKey(e => e.Id);

        b.Property(e => e.Kind).IsRequired();
        b.Property(e => e.Status).IsRequired();
        b.Property(e => e.StartedAtUtc).IsRequired();
        b.Property(e => e.Notes).HasMaxLength(1000);
        b.Property(e => e.IsUrgent).HasDefaultValue(false);

        b.HasIndex(e => new { e.PatientId, e.StartedAtUtc });
        b.HasIndex(e => new { e.Kind, e.Status, e.StartedAtUtc });

        b.HasOne(e => e.Patient)
            .WithMany(p => p.Encounters)
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.Appointment)
            .WithOne(a => a.Encounter)
            .HasForeignKey<Encounter>(e => e.AppointmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Encounter and its detail row form one aggregate, but deletion in this
        // application is soft deletion. Database cascades would bypass the audit trail,
        // so physical deletes are deliberately restricted.
        b.HasOne(e => e.Visit)
            .WithOne(v => v.Encounter)
            .HasForeignKey<Visit>(v => v.EncounterId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.Endoscopy)
            .WithOne(x => x.Encounter)
            .HasForeignKey<Endoscopy>(x => x.EncounterId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.Exam)
            .WithOne(x => x.Encounter)
            .HasForeignKey<Exam>(x => x.EncounterId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.Infai)
            .WithOne(x => x.Encounter)
            .HasForeignKey<InfaiTest>(x => x.EncounterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
