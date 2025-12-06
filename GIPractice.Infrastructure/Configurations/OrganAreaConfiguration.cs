using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class OrganAreaConfiguration : IEntityTypeConfiguration<OrganArea>
{
    public void Configure(EntityTypeBuilder<OrganArea> b)
    {
        b.ToTable("OrganAreas");

        b.HasKey(oa => oa.Id);

        b.Property(oa => oa.Code)
            .IsRequired()
            .HasMaxLength(50);

        b.HasIndex(oa => oa.Code).IsUnique();

        b.Property(oa => oa.DefaultName)
            .IsRequired()
            .HasMaxLength(200);

        b.HasData(
            new OrganArea { Id = 1, Code = "ESOPHAGUS_PROX", DefaultName = "Esophagus proximal third" },
            new OrganArea { Id = 2, Code = "ESOPHAGUS_MID", DefaultName = "Esophagus middle third" },
            new OrganArea { Id = 3, Code = "ESOPHAGUS_DIST", DefaultName = "Esophagus distal third" },
            new OrganArea { Id = 4, Code = "GEJ", DefaultName = "Gastroesophageal junction" },
            new OrganArea { Id = 5, Code = "STOMACH_FUNDUS", DefaultName = "Stomach fundus" },
            new OrganArea { Id = 6, Code = "STOMACH_BODY", DefaultName = "Stomach body" },
            new OrganArea { Id = 7, Code = "STOMACH_INCISURA", DefaultName = "Stomach incisura" },
            new OrganArea { Id = 8, Code = "STOMACH_ANTRUM", DefaultName = "Stomach antrum" },
            new OrganArea { Id = 9, Code = "PYLORUS", DefaultName = "Pylorus" },
            new OrganArea { Id = 10, Code = "DUODENUM_BULB", DefaultName = "Duodenum bulb" },
            new OrganArea { Id = 11, Code = "DUODENUM_SECOND", DefaultName = "Duodenum second part" },
            new OrganArea { Id = 12, Code = "DUODENUM_THIRD", DefaultName = "Duodenum third part (horizontal)" },
            new OrganArea { Id = 13, Code = "CECUM", DefaultName = "Cecum" },
            new OrganArea { Id = 14, Code = "ILEOCECAL_VALVE", DefaultName = "Ileocecal valve" },
            new OrganArea { Id = 15, Code = "COLON_ASC", DefaultName = "Colon ascending" },
            new OrganArea { Id = 16, Code = "COLON_HEP_FLEX", DefaultName = "Colon hepatic flexure" },
            new OrganArea { Id = 17, Code = "COLON_TRANS", DefaultName = "Colon transverse" },
            new OrganArea { Id = 18, Code = "COLON_SPL_FLEX", DefaultName = "Colon splenic flexure" },
            new OrganArea { Id = 19, Code = "COLON_DESC", DefaultName = "Colon descending" },
            new OrganArea { Id = 20, Code = "COLON_SIGMOID", DefaultName = "Colon sigmoid" },
            new OrganArea { Id = 21, Code = "RECTUM_AREA", DefaultName = "Rectum" },
            new OrganArea { Id = 22, Code = "ANAL_CANAL", DefaultName = "Anal canal" }
        );
    }
}