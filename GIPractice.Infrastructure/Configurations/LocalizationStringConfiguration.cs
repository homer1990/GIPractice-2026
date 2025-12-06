using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class LocalizationStringConfiguration : IEntityTypeConfiguration<LocalizationString>
{
    public void Configure(EntityTypeBuilder<LocalizationString> b)
    {
        b.ToTable("LocalizationStrings");

        b.HasKey(x => x.Id);

        b.Property(x => x.Domain)
            .IsRequired()
            .HasMaxLength(100);

        b.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(100);

        b.Property(x => x.Language)
            .IsRequired()
            .HasMaxLength(10);

        b.Property(x => x.Text)
            .IsRequired()
            .HasMaxLength(500);

        // One translation per (Domain, Code, Language)
        b.HasIndex(x => new { x.Domain, x.Code, x.Language }).IsUnique();
        b.HasData(
            new LocalizationString
            {
                Id = 1,
                Domain = "Organ",
                Code = "STOMACH",
                Language = "el",
                Text = "Στομάχι"
            },
            new LocalizationString
            {
                Id = 2,
                Domain = "OrganArea",
                Code = "GEJ",
                Language = "el",
                Text = "Γαστροοισοφαγική συμβολή"
            },
            // add more as desired
            new LocalizationString
            {
                Id = 1001,
                Domain = "EndoscopyType",
                Code = "Gastroscopy",
                Language = "el",
                Text = "Γαστροσκόπηση"
            },
            new LocalizationString
            {
                Id = 1002,
                Domain = "EndoscopyType",
                Code = "Colonoscopy",
                Language = "el",
                Text = "Κολονοσκόπηση"
            },
            new LocalizationString
            {
                Id = 2001,
                Domain = "TestType",
                Code = "H_Pylori_UreaBreath",
                Language = "el",
                Text = "Δοκιμασία αναπνοής ουρίας για H. pylori"
            },
            new LocalizationString
            {
                Id = 2002,
                Domain = "TestType",
                Code = "FecalOccultBlood",
                Language = "el",
                Text = "Απόκρυφο αίμα κοπράνων"
            },

            // --- OperationType translations (examples) ---
            new LocalizationString
            {
                Id = 2101,
                Domain = "OperationType",
                Code = "Polypectomy",
                Language = "el",
                Text = "Πολυπεκτομή"
            },
            new LocalizationString
            {
                Id = 2102,
                Domain = "OperationType",
                Code = "HemostasisClip",
                Language = "el",
                Text = "Αιμόσταση με κλιπ"
            }
        );
    }
}
