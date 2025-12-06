using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class OrganAreaOrganConfiguration : IEntityTypeConfiguration<OrganAreaOrgan>
{
    public void Configure(EntityTypeBuilder<OrganAreaOrgan> b)
    {
        b.ToTable("OrganAreaOrgans");

        b.HasKey(x => new { x.OrganId, x.OrganAreaId });

        b.HasOne(x => x.Organ)
            .WithMany(o => o.OrganAreaOrgans)
            .HasForeignKey(x => x.OrganId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.OrganArea)
            .WithMany(oa => oa.OrganAreaOrgans)
            .HasForeignKey(x => x.OrganAreaId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasData(
            // Esophagus segments
            new OrganAreaOrgan { OrganId = 1, OrganAreaId = 1 },
            new OrganAreaOrgan { OrganId = 1, OrganAreaId = 2 },
            new OrganAreaOrgan { OrganId = 1, OrganAreaId = 3 },

            // GEJ touches esophagus + stomach
            new OrganAreaOrgan { OrganId = 1, OrganAreaId = 4 },
            new OrganAreaOrgan { OrganId = 2, OrganAreaId = 4 },

            // Stomach areas
            new OrganAreaOrgan { OrganId = 2, OrganAreaId = 5 },
            new OrganAreaOrgan { OrganId = 2, OrganAreaId = 6 },
            new OrganAreaOrgan { OrganId = 2, OrganAreaId = 7 },
            new OrganAreaOrgan { OrganId = 2, OrganAreaId = 8 },
            new OrganAreaOrgan { OrganId = 2, OrganAreaId = 9 },

            // Duodenum
            new OrganAreaOrgan { OrganId = 3, OrganAreaId = 10 },
            new OrganAreaOrgan { OrganId = 3, OrganAreaId = 11 },
            new OrganAreaOrgan { OrganId = 3, OrganAreaId = 12 },

            // Cecum + ileocecal valve
            new OrganAreaOrgan { OrganId = 6, OrganAreaId = 13 }, // colon – cecum
            new OrganAreaOrgan { OrganId = 5, OrganAreaId = 14 }, // ileum – ileocecal valve
            new OrganAreaOrgan { OrganId = 6, OrganAreaId = 14 }, // colon – ileocecal valve

            // Colon segments
            new OrganAreaOrgan { OrganId = 6, OrganAreaId = 15 },
            new OrganAreaOrgan { OrganId = 6, OrganAreaId = 16 },
            new OrganAreaOrgan { OrganId = 6, OrganAreaId = 17 },
            new OrganAreaOrgan { OrganId = 6, OrganAreaId = 18 },
            new OrganAreaOrgan { OrganId = 6, OrganAreaId = 19 },
            new OrganAreaOrgan { OrganId = 6, OrganAreaId = 20 },

            // Rectum & anal canal
            new OrganAreaOrgan { OrganId = 7, OrganAreaId = 21 },
            new OrganAreaOrgan { OrganId = 8, OrganAreaId = 22 }
        );
    }
}