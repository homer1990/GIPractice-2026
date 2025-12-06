// GIPractice.Infrastructure/Configurations/BiopsyBottleConfiguration.cs

using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BiopsyBottleConfiguration : IEntityTypeConfiguration<BiopsyBottle>
{
    public void Configure(EntityTypeBuilder<BiopsyBottle> b)
    {
        b.ToTable("BiopsyBottles");

        b.HasKey(bb => bb.Id);

        b.Property(bb => bb.Label)
            .IsRequired()
            .HasMaxLength(150);

        b.Property(bb => bb.Number)
            .IsRequired();

        b.Property(bb => bb.CollectedAtUtc)
            .IsRequired();

        // Bottle -> Endoscopy (no cascade delete; we’ll control deletion in API)
        b.HasOne(bb => bb.Endoscopy)
            .WithMany(e => e.BiopsyBottles)
            .HasForeignKey(bb => bb.EndoscopyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Bottle -> Patient
        b.HasOne(bb => bb.Patient)
            .WithMany(p => p.BiopsyBottles)
            .HasForeignKey(bb => bb.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Many-to-many with OrganArea configured on OrganAreaConfiguration side
    }
}
