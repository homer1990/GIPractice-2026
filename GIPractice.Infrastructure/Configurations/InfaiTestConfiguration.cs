using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class InfaiTestConfiguration : IEntityTypeConfiguration<InfaiTest>
{
    public void Configure(EntityTypeBuilder<InfaiTest> b)
    {
        b.ToTable("InfaiTests");

        b.HasKey(i => i.Id);

        b.Property(i => i.PerformedAtUtc)
            .IsRequired();

        b.Property(i => i.Notes)
            .HasMaxLength(500);

        b.HasOne(i => i.Patient)
            .WithMany(p => p.InfaiTests)
            .HasForeignKey(i => i.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(i => i.File)
            .WithMany()
            .HasForeignKey(i => i.FileId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
