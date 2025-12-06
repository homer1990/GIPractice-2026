using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class TestConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> b)
    {
        b.ToTable("Tests");

        b.HasKey(t => t.Id);

        b.Property(t => t.PerformedAtUtc)
            .IsRequired();

        b.Property(t => t.Notes)
            .HasMaxLength(500);

        b.Property(t => t.QuantitativeResult)
            .HasColumnType("decimal(18,4)");

        b.HasOne(t => t.Patient)
            .WithMany(p => p.Tests)
            .HasForeignKey(t => t.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(t => t.Lab)
            .WithMany(l => l.Tests)
            .HasForeignKey(t => t.LabId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(t => t.Doctor)
            .WithMany(d => d.Tests)
            .HasForeignKey(t => t.DoctorId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(t => t.File)
            .WithMany()
            .HasForeignKey(t => t.FileId)
            .OnDelete(DeleteBehavior.SetNull);

        // Many-to-many Test <-> Finding
        b.HasMany(t => t.Findings)
            .WithMany(f => f.Tests)
            .UsingEntity<Dictionary<string, object>>(
                "TestFinding",
                j => j
                    .HasOne<Finding>()
                    .WithMany()
                    .HasForeignKey("FindingId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Test>()
                    .WithMany()
                    .HasForeignKey("TestId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("TestFindings");
                    j.HasKey("TestId", "FindingId");
                });
    }
}
