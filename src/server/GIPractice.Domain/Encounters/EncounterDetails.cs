namespace GIPractice.Domain.Encounters;

public interface IEncounterDetail
{
    EncounterId EncounterId { get; }
}

public enum VisitKind
{
    Consultation = 1,
    Prescription = 2,
    PreparationInstructions = 3,
    FollowUp = 4,
    Other = 99
}

public sealed class Visit : IEncounterDetail
{
    public EncounterId EncounterId { get; }
    public VisitKind Kind { get; private set; }

    public Visit(EncounterId encounterId, VisitKind kind)
    {
        EncounterId = encounterId;
        Kind = kind;
    }

    public void ChangeKind(VisitKind kind) => Kind = kind;
}

public enum EndoscopyType
{
    Gastroscopy = 1,
    Colonoscopy = 2,
    GastroscopyAndColonoscopy = 3,
    Rectoscopy = 4,
    Other = 99
}

public sealed class Endoscopy : IEncounterDetail
{
    public EncounterId EncounterId { get; }
    public EndoscopyType Type { get; private set; }
    public string? ReportDocumentJson { get; private set; }

    public Endoscopy(EncounterId encounterId, EndoscopyType type)
    {
        EncounterId = encounterId;
        Type = type;
    }

    public void ChangeType(EndoscopyType type) => Type = type;
    public void SetReportDocument(string? json) => ReportDocumentJson = string.IsNullOrWhiteSpace(json) ? null : json;
}

public sealed class ClinicalExam : IEncounterDetail
{
    public EncounterId EncounterId { get; }
    public bool HasSeriousFindings { get; private set; }
    public string? ClinicalNotes { get; private set; }

    public ClinicalExam(EncounterId encounterId, bool hasSeriousFindings = false, string? clinicalNotes = null)
    {
        EncounterId = encounterId;
        HasSeriousFindings = hasSeriousFindings;
        SetClinicalNotes(clinicalNotes);
    }

    public void SetSeriousFindings(bool value) => HasSeriousFindings = value;

    public void SetClinicalNotes(string? notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
        {
            ClinicalNotes = null;
            return;
        }

        var normalized = notes.Trim();
        if (normalized.Length > 16000) throw new ArgumentOutOfRangeException(nameof(notes));
        ClinicalNotes = normalized;
    }
}

public enum InfaiResult
{
    Pending = 0,
    Negative = 1,
    Positive = 2,
    Indeterminate = 3
}

public sealed class InfaiTest : IEncounterDetail
{
    public EncounterId EncounterId { get; }
    public InfaiResult Result { get; private set; }
    public bool PatientContacted { get; private set; }
    public string? ReportStorageKey { get; private set; }

    public InfaiTest(EncounterId encounterId)
    {
        EncounterId = encounterId;
        Result = InfaiResult.Pending;
    }

    public void SetResult(InfaiResult result) => Result = result;
    public void SetPatientContacted(bool value) => PatientContacted = value;
    public void AttachReport(string storageKey) => ReportStorageKey = string.IsNullOrWhiteSpace(storageKey)
        ? throw new ArgumentException("Report storage key is required.", nameof(storageKey))
        : storageKey.Trim();
}
