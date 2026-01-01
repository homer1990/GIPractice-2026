using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public sealed class PathologyParcelConfig : IEntityTypeConfiguration<PathologyParcel>
{
    public void Configure(EntityTypeBuilder<PathologyParcel> b)
    {
        b.ToTable("PathologyParcels");

        b.HasKey(x => x.Id);

        b.Property(x => x.ParcelCode).IsRequired().HasMaxLength(32);

        b.Property(x => x.CourierName).HasMaxLength(100);
        b.Property(x => x.TrackingNumber).HasMaxLength(100);
        b.Property(x => x.Notes).HasMaxLength(500);

        b.Property(x => x.MonetarySum)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m);

        b.Property(x => x.RowVersion).IsRowVersion();

        // Business key: per-pathologist parcel code uniqueness
        b.HasIndex(x => new { x.PathologistId, x.ParcelCode }).IsUnique();

        b.HasOne(x => x.Pathologist)
            .WithMany()
            .HasForeignKey(x => x.PathologistId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Reports)
            .WithOne(x => x.Parcel)
            .HasForeignKey(x => x.PathologyParcelId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
