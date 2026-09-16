namespace GIPractice.Server.Clinical;

public sealed record LocalizedAnatomicalTerm(
    string Locale,
    string DisplayName,
    IReadOnlyList<string>? Aliases = null);

public sealed record AnatomicalSiteDefinition(
    string Code,
    AnatomicalSiteKind Kind,
    string? ParentCode,
    IReadOnlyList<LocalizedAnatomicalTerm> Localizations);

// Initial GI vocabulary for the anatomy actually used by this practice.
// Greek (el-GR) is the reference/primary authored vocabulary. Stable concept codes are
// language-neutral, so additional languages can be added without changing clinical data.
public static class GiAnatomyVocabulary
{
    public const string PrimaryLocale = ClinicalLocales.GreekGreece;

    public static IReadOnlyList<AnatomicalSiteDefinition> Definitions { get; } =
        new AnatomicalSiteDefinition[]
        {
            Site("ESOPHAGUS", AnatomicalSiteKind.Organ,
                "Οισοφάγος", "Esophagus",
                greekAliases: new[] { "οισοφαγος" },
                englishAliases: new[] { "oesophagus" }),

            Site("ESOPHAGUS_UPPER", AnatomicalSiteKind.Region,
                "Άνω οισοφάγος", "Upper esophagus", "ESOPHAGUS",
                greekAliases: new[] { "άνω οισοφάγος", "ανω οισοφαγος" },
                englishAliases: new[] { "upper oesophagus" }),

            Site("ESOPHAGUS_MIDDLE", AnatomicalSiteKind.Region,
                "Μέσος οισοφάγος", "Middle esophagus", "ESOPHAGUS",
                greekAliases: new[] { "μέσος οισοφάγος", "μεσος οισοφαγος" },
                englishAliases: new[] { "mid esophagus", "mid oesophagus" }),

            Site("ESOPHAGUS_DISTAL", AnatomicalSiteKind.Region,
                "Κάτω οισοφάγος", "Distal esophagus", "ESOPHAGUS",
                greekAliases: new[] { "κάτω οισοφάγος", "κατω οισοφαγος", "distal" },
                englishAliases: new[] { "lower esophagus", "distal oesophagus" }),

            Site("GEJ", AnatomicalSiteKind.Landmark,
                "Γαστροοισοφαγική συμβολή", "Gastroesophageal junction", "ESOPHAGUS",
                greekAliases: new[] { "ΓΟΣ", "GEJ", "γαστροοισοφαγική ένωση", "γαστροοισοφαγικη συμβολη" },
                englishAliases: new[] { "GEJ", "gastro-esophageal junction" }),

            Site("Z_LINE", AnatomicalSiteKind.Landmark,
                "Γραμμή Z", "Z-line", "ESOPHAGUS",
                greekAliases: new[] { "γραμμή z", "γραμμη z", "z-line", "z line" },
                englishAliases: new[] { "Z line", "squamocolumnar junction" }),

            Site("DIAPHRAGMATIC_IMPRESSION", AnatomicalSiteKind.Landmark,
                "Διαφραγματικό εντύπωμα", "Diaphragmatic impression", "ESOPHAGUS",
                greekAliases: new[] { "διαφραγματική εντύπωση", "διαφραγματικο εντυπωμα", "diaphragmatic pinch" },
                englishAliases: new[] { "diaphragmatic depression", "diaphragmatic pinch" }),

            Site("STOMACH", AnatomicalSiteKind.Organ,
                "Στόμαχος", "Stomach",
                greekAliases: new[] { "στομάχι", "στομαχος", "gastric" },
                englishAliases: new[] { "gastric" }),

            Site("STOMACH_CARDIA", AnatomicalSiteKind.Region,
                "Καρδία", "Cardia", "STOMACH",
                greekAliases: new[] { "καρδία στομάχου", "καρδια", "cardia" },
                englishAliases: new[] { "gastric cardia" }),

            Site("STOMACH_FUNDUS", AnatomicalSiteKind.Region,
                "Θόλος στομάχου", "Fundus", "STOMACH",
                greekAliases: new[] { "θόλος", "θολος", "fundus" },
                englishAliases: new[] { "gastric fundus", "fundus" }),

            Site("STOMACH_CORPUS", AnatomicalSiteKind.Region,
                "Σώμα στομάχου", "Gastric body", "STOMACH",
                greekAliases: new[] { "σώμα", "σωμα", "corpus", "body" },
                englishAliases: new[] { "body", "corpus", "stomach body" }),

            Site("STOMACH_INCISURA", AnatomicalSiteKind.Landmark,
                "Γωνιακή εντομή", "Incisura angularis", "STOMACH",
                greekAliases: new[] { "γωνιακή", "γωνιακη εντομη", "incisura", "angulus" },
                englishAliases: new[] { "incisura", "angulus" }),

            Site("STOMACH_ANTRUM", AnatomicalSiteKind.Region,
                "Άντρο στομάχου", "Gastric antrum", "STOMACH",
                greekAliases: new[] { "άντρο", "αντρο", "antrum", "antral" },
                englishAliases: new[] { "antrum", "gastric antrum", "antral" }),

            Site("PYLORUS", AnatomicalSiteKind.Landmark,
                "Πυλωρός", "Pylorus", "STOMACH",
                greekAliases: new[] { "πυλωρος", "pylorus" },
                englishAliases: new[] { "pyloric ring" }),

            Site("DUODENUM", AnatomicalSiteKind.Organ,
                "Δωδεκαδάκτυλο", "Duodenum",
                greekAliases: new[] { "δωδεκαδακτυλο", "duodenum" }),

            Site("DUODENUM_BULB", AnatomicalSiteKind.Region,
                "Βολβός δωδεκαδακτύλου", "Duodenal bulb", "DUODENUM",
                greekAliases: new[] { "βολβός", "βολβος", "D1", "d1", "bulb" },
                englishAliases: new[] { "bulb", "D1" }),

            Site("DUODENUM_D2", AnatomicalSiteKind.Region,
                "2η μοίρα δωδεκαδακτύλου", "Second duodenal portion", "DUODENUM",
                greekAliases: new[] { "2η μοίρα", "2η μοιρα", "D2", "d2" },
                englishAliases: new[] { "D2", "second part duodenum" }),

            Site("TERMINAL_ILEUM", AnatomicalSiteKind.Region,
                "Τελικός ειλεός", "Terminal ileum",
                greekAliases: new[] { "τελικός ειλεός", "τελικος ειλεος", "TI", "terminal ileum" },
                englishAliases: new[] { "TI" }),

            Site("COLON", AnatomicalSiteKind.Organ,
                "Παχύ έντερο", "Colon",
                greekAliases: new[] { "κόλον", "κολον", "colon" },
                englishAliases: new[] { "large bowel" }),

            Site("CECUM", AnatomicalSiteKind.Region,
                "Τυφλό", "Cecum", "COLON",
                greekAliases: new[] { "τυφλό", "τυφλο", "cecum", "caecum" },
                englishAliases: new[] { "caecum" }),

            Site("ILEOCECAL_VALVE", AnatomicalSiteKind.Landmark,
                "Ειλεοτυφλική βαλβίδα", "Ileocecal valve", "COLON",
                greekAliases: new[] { "ειλεοτυφλική", "ειλεοτυφλικη βαλβιδα", "IC valve" },
                englishAliases: new[] { "IC valve" }),

            Site("ASCENDING_COLON", AnatomicalSiteKind.Region,
                "Ανιόν κόλον", "Ascending colon", "COLON",
                greekAliases: new[] { "ανιόν", "ανιον", "ascending" },
                englishAliases: new[] { "ascending" }),

            Site("HEPATIC_FLEXURE", AnatomicalSiteKind.Landmark,
                "Ηπατική καμπή", "Hepatic flexure", "COLON",
                greekAliases: new[] { "ηπατική", "ηπατικη καμπη", "hepatic flexure" },
                englishAliases: new[] { "right colic flexure" }),

            Site("TRANSVERSE_COLON", AnatomicalSiteKind.Region,
                "Εγκάρσιο κόλον", "Transverse colon", "COLON",
                greekAliases: new[] { "εγκάρσιο", "εγκαρσιο", "transverse" },
                englishAliases: new[] { "transverse" }),

            Site("SPLENIC_FLEXURE", AnatomicalSiteKind.Landmark,
                "Σπληνική καμπή", "Splenic flexure", "COLON",
                greekAliases: new[] { "σπληνική", "σπληνικη καμπη", "splenic flexure" },
                englishAliases: new[] { "left colic flexure" }),

            Site("DESCENDING_COLON", AnatomicalSiteKind.Region,
                "Κατιόν κόλον", "Descending colon", "COLON",
                greekAliases: new[] { "κατιόν", "κατιον", "descending" },
                englishAliases: new[] { "descending" }),

            Site("SIGMOID_COLON", AnatomicalSiteKind.Region,
                "Σιγμοειδές κόλον", "Sigmoid colon", "COLON",
                greekAliases: new[] { "σιγμοειδές", "σιγμοειδες", "sigmoid" },
                englishAliases: new[] { "sigmoid" }),

            Site("RECTUM", AnatomicalSiteKind.Region,
                "Ορθό", "Rectum", "COLON",
                greekAliases: new[] { "ορθό", "ορθο", "rectum" })
        };

    private static AnatomicalSiteDefinition Site(
        string code,
        AnatomicalSiteKind kind,
        string greekDisplayName,
        string englishDisplayName,
        string? parentCode = null,
        IReadOnlyList<string>? greekAliases = null,
        IReadOnlyList<string>? englishAliases = null) =>
        new(
            code,
            kind,
            parentCode,
            new LocalizedAnatomicalTerm[]
            {
                new(ClinicalLocales.GreekGreece, greekDisplayName, greekAliases),
                new(ClinicalLocales.English, englishDisplayName, englishAliases)
            });
}
