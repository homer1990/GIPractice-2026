namespace GIPractice.Server.Clinical;

public sealed record ClinicalExam(
    Guid Id,
    ClinicalDocument Examination,
    string? Assessment = null)
{
    public static ClinicalExam Create(string? examinationText, string? assessment = null) =>
        new(
            Guid.CreateVersion7(),
            ClinicalDocument.FromText(examinationText),
            Normalize(assessment));

    private static string? Normalize(string? text) =>
        string.IsNullOrWhiteSpace(text) ? null : text.Trim();
}
