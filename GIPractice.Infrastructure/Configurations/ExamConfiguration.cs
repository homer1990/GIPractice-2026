using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public sealed class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> b)
    {
        b.ToTable("Exams");
        b.HasKey(e => e.Id);
        b.HasIndex(e => e.EncounterId).IsUnique();
        b.Property(e => e.Notes).HasMaxLength(2000);
        b.Property(e => e.SeriousFindings).HasDefaultValue(false);
    }
}
