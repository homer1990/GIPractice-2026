using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class DiagnosisConfiguration : IEntityTypeConfiguration<Diagnosis>
{
    public void Configure(EntityTypeBuilder<Diagnosis> b)
    {
        b.ToTable("Diagnoses");

        b.HasKey(d => d.Id);

        b.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(50);

        b.Property(d => d.Code)
            .HasMaxLength(50);

        b.HasIndex(d => new { d.Code, d.Standard });

        // Many-to-many with Patient
        b.HasMany(d => d.Patients)
            .WithMany(p => p.Diagnoses)
            .UsingEntity<Dictionary<string, object>>(
                "PatientDiagnosis",
                j => j
                    .HasOne<Patient>()
                    .WithMany()
                    .HasForeignKey("PatientId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Diagnosis>()
                    .WithMany()
                    .HasForeignKey("DiagnosisId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("PatientDiagnoses");
                    j.HasKey("PatientId", "DiagnosisId");
                });

        // Many-to-many with Visit
        b.HasMany(d => d.Visits)
            .WithMany(v => v.Diagnoses)
            .UsingEntity<Dictionary<string, object>>(
                "VisitDiagnosis",
                j => j
                    .HasOne<Visit>()
                    .WithMany()
                    .HasForeignKey("VisitId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Diagnosis>()
                    .WithMany()
                    .HasForeignKey("DiagnosisId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("VisitDiagnoses");
                    j.HasKey("VisitId", "DiagnosisId");
                });

        // Many-to-many with Endoscopy
        b.HasMany(d => d.Endoscopies)
            .WithMany(e => e.Diagnoses)
            .UsingEntity<Dictionary<string, object>>(
                "EndoscopyDiagnosis",
                j => j
                    .HasOne<Endoscopy>()
                    .WithMany()
                    .HasForeignKey("EndoscopyId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Diagnosis>()
                    .WithMany()
                    .HasForeignKey("DiagnosisId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("EndoscopyDiagnoses");
                    j.HasKey("EndoscopyId", "DiagnosisId");
                });

        // Many-to-many with Treatment configured on Treatment side
    }
}
