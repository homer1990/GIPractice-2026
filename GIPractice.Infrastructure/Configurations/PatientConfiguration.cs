using GIPractice.Core.Entities;
using GIPractice.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> b)
    {
        b.ToTable("Patients", tb =>
        {
            // SQL Server-style CHECK constraint:
            tb.HasCheckConstraint(
                "CK_Patients_PersonalNumber_Format",
                "[PersonalNumber] IS NOT NULL " +
                "AND LEN([PersonalNumber]) = 12 " +
                "AND [PersonalNumber] NOT LIKE '%[^0-9]%'"
            );
        });

        b.HasKey(p => p.Id);

        b.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(30);

        b.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(30);

        b.Property(p => p.FathersName)
            .IsRequired()
            .HasMaxLength(30);

        // PersonalNumber value object <-> string
        b.Property(p => p.PersonalNumber)
            .IsRequired()
            .HasConversion(
                pn => pn.Value,
                s => PersonalNumber.Create(s)
            )
            .HasMaxLength(12);

        b.HasIndex(p => p.PersonalNumber)
            .IsUnique();

        b.Property(p => p.Email)
            .HasMaxLength(100);

        b.Property(p => p.PhoneNumber)
            .HasMaxLength(20);

        b.Property(p => p.Address)
            .HasMaxLength(250);

        // Relationships are mostly convention-based; this is enough:
        b.HasMany(p => p.Appointments)
            .WithOne(a => a.Patient)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(p => p.Visits)
            .WithOne(v => v.Patient)
            .HasForeignKey(v => v.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(p => p.Endoscopies)
            .WithOne(e => e.Patient)
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
