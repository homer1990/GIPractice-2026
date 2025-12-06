using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class PreparationProtocolConfiguration : IEntityTypeConfiguration<PreparationProtocol>
{
    public void Configure(EntityTypeBuilder<PreparationProtocol> b)
    {
        b.ToTable("PreparationProtocols");

        b.HasKey(p => p.Id);

        b.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        b.Property(p => p.Instructions)
            .IsRequired();

        b.Property(p => p.MinimumFastingHours);

        b.Property(p => p.IncludesBowelCleaning)
            .HasDefaultValue(false);

        b.Property(p => p.IsActive)
            .HasDefaultValue(true);
    }
}
