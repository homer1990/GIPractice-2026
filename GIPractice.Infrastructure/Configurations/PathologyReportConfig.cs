using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public sealed class PathologyReportConfig : IEntityTypeConfiguration<PathologyReport>
{
    public void Configure(EntityTypeBuilder<PathologyReport> b)
    {
        b.Property(x => x.ClinicalInfo).HasMaxLength(2000);
        b.Property(x => x.MacroscopyText).HasMaxLength(8000);
        b.Property(x => x.DiagnosisText).HasMaxLength(8000);
        b.Property(x => x.Notes).HasMaxLength(2000);

        b.Property(x => x.RowVersion).IsRowVersion();

        // Optional, but likely what you want: one report per endoscopy per pathologist
        b.HasIndex(x => new { x.PathologistId, x.EndoscopyId }).IsUnique();

        b.HasOne(x => x.DocumentFile)
            .WithMany()
            .HasForeignKey(x => x.DocumentFileId);
    }
}
