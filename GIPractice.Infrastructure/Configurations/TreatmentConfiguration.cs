using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class TreatmentConfiguration : IEntityTypeConfiguration<Treatment>
{
    public void Configure(EntityTypeBuilder<Treatment> b)
    {
        b.ToTable("Treatments");

        b.HasKey(t => t.Id);

        b.Property(t => t.StartUtc)
            .IsRequired();

        b.Property(t => t.Notes)
            .HasMaxLength(1000);

        b.Property(t => t.IsChronic)
            .HasDefaultValue(false);

        b.HasOne(t => t.Patient)
            .WithMany(p => p.Treatments)
            .HasForeignKey(t => t.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(t => t.Doctor)
            .WithMany(d => d.Treatments)
            .HasForeignKey(t => t.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Many-to-many Treatment <-> Diagnosis
        b.HasMany(t => t.Diagnoses)
            .WithMany(d => d.Treatments)
            .UsingEntity<Dictionary<string, object>>(
                "TreatmentDiagnosis",
                j => j
                    .HasOne<Diagnosis>()
                    .WithMany()
                    .HasForeignKey("DiagnosisId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Treatment>()
                    .WithMany()
                    .HasForeignKey("TreatmentId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("TreatmentDiagnoses");
                    j.HasKey("TreatmentId", "DiagnosisId");
                });

        // Many-to-many Treatment <-> Medicine
        b.HasMany(t => t.Medications)
            .WithMany(m => m.Treatments)
            .UsingEntity<Dictionary<string, object>>(
                "TreatmentMedicine",
                j => j
                    .HasOne<Medicine>()
                    .WithMany()
                    .HasForeignKey("MedicineId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Treatment>()
                    .WithMany()
                    .HasForeignKey("TreatmentId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("TreatmentMedicines");
                    j.HasKey("TreatmentId", "MedicineId");
                });
    }
}
