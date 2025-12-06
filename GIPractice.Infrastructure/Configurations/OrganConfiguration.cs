using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class OrganConfiguration : IEntityTypeConfiguration<Organ>
{
    public void Configure(EntityTypeBuilder<Organ> b)
    {
        b.ToTable("Organs");

        b.HasKey(o => o.Id);

        b.Property(o => o.Code)
            .IsRequired()
            .HasMaxLength(50);

        b.HasIndex(o => o.Code).IsUnique();

        b.Property(o => o.DefaultName)
            .IsRequired()
            .HasMaxLength(200);

        b.HasData(
            new Organ { Id = 1, Code = "ESOPHAGUS", DefaultName = "Esophagus" },
            new Organ { Id = 2, Code = "STOMACH", DefaultName = "Stomach" },
            new Organ { Id = 3, Code = "DUODENUM", DefaultName = "Duodenum" },
            new Organ { Id = 4, Code = "JEJUNUM", DefaultName = "Jejunum" },
            new Organ { Id = 5, Code = "ILEUM", DefaultName = "Ileum" },
            new Organ { Id = 6, Code = "COLON", DefaultName = "Colon" },
            new Organ { Id = 7, Code = "RECTUM", DefaultName = "Rectum" },
            new Organ { Id = 8, Code = "ANUS", DefaultName = "Anus" }
        );
    }
}
