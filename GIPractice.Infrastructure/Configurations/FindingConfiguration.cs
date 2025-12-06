using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class FindingConfiguration : IEntityTypeConfiguration<Finding>
{
    public void Configure(EntityTypeBuilder<Finding> b)
    {
        b.ToTable("Findings");

        b.HasKey(f => f.Id);

        b.Property(f => f.Code)
            .IsRequired()
            .HasMaxLength(50);

        b.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(500);

        b.Property(f => f.Standard)
            .IsRequired()
            .HasMaxLength(100);

        // Observations relation configured in ObservationConfiguration
        // Many-to-many to Endoscopy/Test/InfaiTest, if needed, can be added later
    }
}
