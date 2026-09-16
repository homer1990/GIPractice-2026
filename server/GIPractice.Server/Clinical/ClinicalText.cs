namespace GIPractice.Server.Clinical;

public enum ClinicalAnnotationKind
{
    Diagnosis = 1,
    Finding = 2,
    Symptom = 3,
    Medication = 4,
    Anatomy = 5,
    PatientReference = 6,
    Other = 99
}

public sealed record ClinicalTextAnnotation(
    Guid Id,
    int Start,
    int Length,
    ClinicalAnnotationKind Kind,
    string? CodeSystem = null,
    string? Code = null,
    Guid? TargetPatientId = null,
    bool ConfirmedByUser = false)
{
    public int End => Start + Length;
}

public sealed record ClinicalDocument(
    string Text,
    IReadOnlyList<ClinicalTextAnnotation> Annotations)
{
    public static ClinicalDocument FromText(string? text) =>
        new(Normalize(text), Array.Empty<ClinicalTextAnnotation>());

    private static string Normalize(string? text) =>
        string.IsNullOrWhiteSpace(text) ? string.Empty : text.Trim();
}
