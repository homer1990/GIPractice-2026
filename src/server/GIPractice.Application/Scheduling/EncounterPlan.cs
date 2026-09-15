using GIPractice.Domain;
using GIPractice.Domain.Encounters;

namespace GIPractice.Application.Scheduling;

public abstract record EncounterPlan
{
    public abstract EncounterKind Kind { get; }
    internal abstract IEncounterDetail CreateDetail(EncounterId encounterId);
}

public sealed record VisitEncounterPlan(VisitKind VisitKind) : EncounterPlan
{
    public override EncounterKind Kind => EncounterKind.Visit;
    internal override IEncounterDetail CreateDetail(EncounterId encounterId) => new Visit(encounterId, VisitKind);
}

public sealed record EndoscopyEncounterPlan(EndoscopyType EndoscopyType) : EncounterPlan
{
    public override EncounterKind Kind => EncounterKind.Endoscopy;
    internal override IEncounterDetail CreateDetail(EncounterId encounterId) => new Endoscopy(encounterId, EndoscopyType);
}

public sealed record ClinicalExamEncounterPlan(
    bool HasSeriousFindings = false,
    string? ClinicalNotes = null) : EncounterPlan
{
    public override EncounterKind Kind => EncounterKind.ClinicalExam;
    internal override IEncounterDetail CreateDetail(EncounterId encounterId) =>
        new ClinicalExam(encounterId, HasSeriousFindings, ClinicalNotes);
}

public sealed record InfaiEncounterPlan : EncounterPlan
{
    public override EncounterKind Kind => EncounterKind.Infai;
    internal override IEncounterDetail CreateDetail(EncounterId encounterId) => new InfaiTest(encounterId);
}
