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
        b.HasIndex(i => i.EncounterId).IsUnique();
        b.Property(i => i.Notes).HasMaxLength(1000);

        b.HasOne(i => i.File)
            .WithMany()
            .HasForeignKey(i => i.FileId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
