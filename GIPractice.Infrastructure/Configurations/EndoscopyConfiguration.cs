using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class EndoscopyConfiguration : IEntityTypeConfiguration<Endoscopy>
{
    public void Configure(EntityTypeBuilder<Endoscopy> b)
    {
        b.ToTable("Endoscopies");
        b.HasKey(e => e.Id);
        b.HasIndex(e => e.EncounterId).IsUnique();

        b.Property(e => e.Type).IsRequired();
        b.Property(e => e.Notes).HasMaxLength(2000);
        b.Property(e => e.EndoscopyCost).HasPrecision(18, 2);
        b.Property(e => e.BiopsiesCost).HasPrecision(18, 2);

        b.HasOne(e => e.Report)
            .WithOne(r => r.Endoscopy)
            .HasForeignKey<Report>(r => r.EndoscopyId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(e => e.BiopsyBottles)
            .WithOne(bottle => bottle.Endoscopy)
            .HasForeignKey(bottle => bottle.EndoscopyId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(e => e.MediaFiles)
            .WithOne(m => m.Endoscopy)
            .HasForeignKey(m => m.EndoscopyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
