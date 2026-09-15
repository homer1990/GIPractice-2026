using GIPractice.Domain;
using GIPractice.Domain.Encounters;

namespace GIPractice.Application.Scheduling;

public enum EncounterDetailType
{
    Visit = 1,
    Endoscopy = 2,
    ClinicalExam = 3,
    Prescription = 4,
    Infai = 5
}

public abstract record EncounterDetailPlan
{
    public abstract EncounterDetailType Type { get; }
    internal abstract IEncounterDetail CreateDetail(EncounterId encounterId);
}

public sealed record VisitPlan(VisitKind VisitKind) : EncounterDetailPlan
{
    public override EncounterDetailType Type => EncounterDetailType.Visit;
    internal override IEncounterDetail CreateDetail(EncounterId encounterId) => new Visit(encounterId, VisitKind);
}

public sealed record EndoscopyPlan(EndoscopyType EndoscopyType) : EncounterDetailPlan
{
    public override EncounterDetailType Type => EncounterDetailType.Endoscopy;
    internal override IEncounterDetail CreateDetail(EncounterId encounterId) => new Endoscopy(encounterId, EndoscopyType);
}

public sealed record ClinicalExamPlan(
    bool HasSeriousFindings = false,
    string? ClinicalNotes = null) : EncounterDetailPlan
{
    public override EncounterDetailType Type => EncounterDetailType.ClinicalExam;
    internal override IEncounterDetail CreateDetail(EncounterId encounterId) =>
        new ClinicalExam(encounterId, HasSeriousFindings, ClinicalNotes);
}

public sealed record PrescriptionPlan(string? Notes = null) : EncounterDetailPlan
{
    public override EncounterDetailType Type => EncounterDetailType.Prescription;
    internal override IEncounterDetail CreateDetail(EncounterId encounterId) => new Prescription(encounterId, Notes);
}

public sealed record InfaiPlan : EncounterDetailPlan
{
    public override EncounterDetailType Type => EncounterDetailType.Infai;
    internal override IEncounterDetail CreateDetail(EncounterId encounterId) => new InfaiTest(encounterId);
}

public sealed class EncounterPlan
{
    private readonly EncounterDetailPlan[] _details;

    public IReadOnlyList<EncounterDetailPlan> Details => _details;
    public bool RequiresExclusiveSlot => _details.Any(detail => detail.Type != EncounterDetailType.Infai);

    public EncounterPlan(params EncounterDetailPlan[] details)
    {
        ArgumentNullException.ThrowIfNull(details);
        if (details.Length == 0)
            throw new ArgumentException("An encounter must contain at least one clinical component.", nameof(details));
        if (details.Any(detail => detail is null))
            throw new ArgumentException("Encounter components cannot contain null values.", nameof(details));

        var duplicate = details
            .GroupBy(detail => detail.Type)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicate is not null)
            throw new DomainRuleViolationException(
                $"An encounter cannot contain more than one {duplicate.Key} component.");

        _details = [.. details];
    }

    internal IReadOnlyList<IEncounterDetail> CreateDetails(EncounterId encounterId) =>
        _details.Select(detail => detail.CreateDetail(encounterId)).ToArray();
}
