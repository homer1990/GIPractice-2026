using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> b)
    {
        b.ToTable("MediaFiles");

        b.HasKey(m => m.Id);

        b.Property(m => m.FileName)
            .IsRequired()
            .HasMaxLength(255);

        b.Property(m => m.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        b.Property(m => m.PseudoLink)
            .IsRequired()
            .HasMaxLength(2048);

        b.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(256);

        b.Property(m => m.FilePath)
            .IsRequired()
            .HasMaxLength(2048);

        b.Property(m => m.ReceivedAtUtc);

        // TPH for EndoscopyMedia
        b.HasDiscriminator<string>("MediaType")
            .HasValue<MediaFile>("Generic")
            .HasValue<EndoscopyMedia>("Endoscopy");
    }
}
