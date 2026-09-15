using GIPractice.Core.Entities;
using GIPractice.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> b)
    {
        b.ToTable("Patients");
        b.HasKey(p => p.Id);

        b.Property(p => p.FirstName).IsRequired().HasMaxLength(60);
        b.Property(p => p.LastName).IsRequired().HasMaxLength(60);
        b.Property(p => p.FathersName).HasMaxLength(60);

        // Format validation belongs to the value object / input validator. A provider-
        // specific SQL Server CHECK constraint here made the supposedly agnostic model
        // impossible to use unchanged on SQLite, PostgreSQL and MySQL/MariaDB.
        b.Property(p => p.PersonalNumber)
            .IsRequired()
            .HasConversion(pn => pn.Value, value => PersonalNumber.Create(value))
            .HasMaxLength(12);

        b.HasIndex(p => p.PersonalNumber).IsUnique();

        b.Property(p => p.Email).HasMaxLength(254);
        b.Property(p => p.PhoneNumber).HasMaxLength(32);
        b.Property(p => p.Address).HasMaxLength(500);

        b.HasMany(p => p.Appointments)
            .WithOne(a => a.Patient)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(p => p.Encounters)
            .WithOne(e => e.Patient)
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
