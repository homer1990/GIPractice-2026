using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public sealed class PathologyParcelConfig : IEntityTypeConfiguration<PathologyParcel>
{
    public void Configure(EntityTypeBuilder<PathologyParcel> b)
    {
        b.Property(x => x.ParcelCode).HasMaxLength(32).IsRequired();
        b.Property(x => x.CourierName).HasMaxLength(100);
        b.Property(x => x.TrackingNumber).HasMaxLength(100);
        b.Property(x => x.Notes).HasMaxLength(500);

        b.Property(x => x.RowVersion).IsRowVersion();

        // Business key: per-pathologist parcel code uniqueness
        b.HasIndex(x => new { x.PathologistId, x.ParcelCode }).IsUnique();

        b.HasMany(x => x.Reports)
            .WithOne(x => x.Parcel)
            .HasForeignKey(x => x.PathologyParcelId);
    }
}
