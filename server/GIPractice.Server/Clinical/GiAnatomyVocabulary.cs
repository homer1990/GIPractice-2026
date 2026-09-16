namespace GIPractice.Server.Clinical;

public sealed record AnatomicalSiteDefinition(
    string Code,
    string DisplayName,
    AnatomicalSiteKind Kind,
    string? ParentCode = null,
    IReadOnlyList<string>? Aliases = null);

// Initial seed vocabulary for the anatomy we actually use in this practice. This is
// deliberately small and extensible; it is not an attempt to reproduce a complete
// medical ontology.
public static class GiAnatomyVocabulary
{
    public static IReadOnlyList<AnatomicalSiteDefinition> Definitions { get; } =
        new AnatomicalSiteDefinition[]
        {
            new("ESOPHAGUS", "Esophagus", AnatomicalSiteKind.Organ, Aliases: new[] { "oesophagus", "οισοφάγος" }),
            new("ESOPHAGUS_UPPER", "Upper esophagus", AnatomicalSiteKind.Region, "ESOPHAGUS", new[] { "upper oesophagus", "άνω οισοφάγος" }),
            new("ESOPHAGUS_MIDDLE", "Middle esophagus", AnatomicalSiteKind.Region, "ESOPHAGUS", new[] { "mid esophagus", "μέσος οισοφάγος" }),
            new("ESOPHAGUS_DISTAL", "Distal esophagus", AnatomicalSiteKind.Region, "ESOPHAGUS", new[] { "lower esophagus", "distal oesophagus", "κάτω οισοφάγος" }),
            new("GEJ", "Gastroesophageal junction", AnatomicalSiteKind.Landmark, "ESOPHAGUS", new[] { "GEJ", "gastro-esophageal junction", "γαστροοισοφαγική συμβολή" }),
            new("Z_LINE", "Z-line", AnatomicalSiteKind.Landmark, "ESOPHAGUS", new[] { "Z line", "squamocolumnar junction", "γραμμή Z" }),
            new("DIAPHRAGMATIC_IMPRESSION", "Diaphragmatic impression", AnatomicalSiteKind.Landmark, "ESOPHAGUS", new[] { "diaphragmatic depression", "diaphragmatic pinch", "διαφραγματικό εντύπωμα" }),

            new("STOMACH", "Stomach", AnatomicalSiteKind.Organ, Aliases: new[] { "gastric", "στόμαχος" }),
            new("STOMACH_CARDIA", "Cardia", AnatomicalSiteKind.Region, "STOMACH", new[] { "gastric cardia", "καρδία" }),
            new("STOMACH_FUNDUS", "Fundus", AnatomicalSiteKind.Region, "STOMACH", new[] { "gastric fundus", "θόλος", "fundus" }),
            new("STOMACH_CORPUS", "Corpus", AnatomicalSiteKind.Region, "STOMACH", new[] { "body", "gastric body", "σώμα", "corpus" }),
            new("STOMACH_INCISURA", "Incisura angularis", AnatomicalSiteKind.Landmark, "STOMACH", new[] { "incisura", "angulus", "γωνιακή εντομή" }),
            new("STOMACH_ANTRUM", "Antrum", AnatomicalSiteKind.Region, "STOMACH", new[] { "gastric antrum", "άντρο", "antral" }),
            new("PYLORUS", "Pylorus", AnatomicalSiteKind.Landmark, "STOMACH", new[] { "pyloric ring", "πυλωρός" }),

            new("DUODENUM", "Duodenum", AnatomicalSiteKind.Organ, Aliases: new[] { "δωδεκαδάκτυλο" }),
            new("DUODENUM_BULB", "Duodenal bulb", AnatomicalSiteKind.Region, "DUODENUM", new[] { "bulb", "D1", "βολβός" }),
            new("DUODENUM_D2", "Second duodenal portion", AnatomicalSiteKind.Region, "DUODENUM", new[] { "D2", "second part duodenum", "2η μοίρα δωδεκαδακτύλου" }),

            new("TERMINAL_ILEUM", "Terminal ileum", AnatomicalSiteKind.Region, Aliases: new[] { "TI", "τελικός ειλεός" }),

            new("COLON", "Colon", AnatomicalSiteKind.Organ, Aliases: new[] { "large bowel", "παχύ έντερο" }),
            new("CECUM", "Cecum", AnatomicalSiteKind.Region, "COLON", new[] { "caecum", "τυφλό" }),
            new("ILEOCECAL_VALVE", "Ileocecal valve", AnatomicalSiteKind.Landmark, "COLON", new[] { "IC valve", "ειλεοτυφλική βαλβίδα" }),
            new("ASCENDING_COLON", "Ascending colon", AnatomicalSiteKind.Region, "COLON", new[] { "ascending", "ανιόν" }),
            new("HEPATIC_FLEXURE", "Hepatic flexure", AnatomicalSiteKind.Landmark, "COLON", new[] { "right colic flexure", "ηπατική καμπή" }),
            new("TRANSVERSE_COLON", "Transverse colon", AnatomicalSiteKind.Region, "COLON", new[] { "transverse", "εγκάρσιο" }),
            new("SPLENIC_FLEXURE", "Splenic flexure", AnatomicalSiteKind.Landmark, "COLON", new[] { "left colic flexure", "σπληνική καμπή" }),
            new("DESCENDING_COLON", "Descending colon", AnatomicalSiteKind.Region, "COLON", new[] { "descending", "κατιόν" }),
            new("SIGMOID_COLON", "Sigmoid colon", AnatomicalSiteKind.Region, "COLON", new[] { "sigmoid", "σιγμοειδές" }),
            new("RECTUM", "Rectum", AnatomicalSiteKind.Region, "COLON", new[] { "ορθό" })
        };
}
