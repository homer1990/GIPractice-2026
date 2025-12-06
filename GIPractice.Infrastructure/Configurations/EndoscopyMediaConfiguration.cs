using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class EndoscopyMediaConfiguration : IEntityTypeConfiguration<EndoscopyMedia>
{
    public void Configure(EntityTypeBuilder<EndoscopyMedia> b)
    {
        // TPH inherits from MediaFile; table & discriminator configured there.
        b.HasOne(em => em.Endoscopy)
            .WithMany(e => e.MediaFiles)
            .HasForeignKey(em => em.EndoscopyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
