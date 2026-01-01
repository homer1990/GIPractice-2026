using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public sealed class PathologistConfig : IEntityTypeConfiguration<Pathologist>
{
    public void Configure(EntityTypeBuilder<Pathologist> b)
    {
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Email).HasMaxLength(200);
        b.Property(x => x.PhoneNumber).HasMaxLength(50);
        b.Property(x => x.Address).HasMaxLength(500);

        b.Property(x => x.PricingPlanJson).IsRequired();

        b.Property(x => x.RowVersion).IsRowVersion();
    }
}
