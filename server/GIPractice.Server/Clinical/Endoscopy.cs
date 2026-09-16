namespace GIPractice.Server.Clinical;

public enum ProcedurePriority
{
    Routine = 0,
    Urgent = 1,
    Emergency = 2
}

public enum EndoscopyOutcome
{
    InProgress = 0,
    Completed = 1,
    Limited = 2,
    Aborted = 3
}

public enum PreparationMode
{
    NotApplicable = 0,
    Standard = 1,
    PartialEmergency = 2,
    Other = 99
}

public enum SedationMode
{
    None = 0,
    LightSedation = 1,
    AnaesthesiologistPropofol = 2,
    Other = 99
}

public enum EndoscopyPhase
{
    Preparation = 1,
    Sedation = 2,
    Procedure = 3,
    Recovery = 4,
    Debrief = 5,
    Other = 99
}

public enum TerminationReasonKind
{
    PatientIntolerance = 1,
    BradycardiaVagotony = 2,
    RetainedGastricContents = 3,
    InadequatePreparation = 4,
    Obstruction = 5,
    UnsafeAnatomicalFinding = 6,
    Pain = 7,
    UnsafeToContinue = 8,
    Other = 99
}

public sealed record Endoscopy(
    Guid Id,
    string TypeCode,
    DateTimeOffset StartedAtUtc,
    string? Indication = null,
    ProcedurePriority Priority = ProcedurePriority.Routine,
    EndoscopyOutcome Outcome = EndoscopyOutcome.InProgress,
    DateTimeOffset? EndedAtUtc = null,
    string? ExtentReachedCode = null,
    PreparationMode PreparationMode = PreparationMode.NotApplicable,
    string? PreparationQualityCode = null,
    SedationMode SedationMode = SedationMode.None,
    string? Impression = null,
    string? Recommendations = null)
{
    public Endoscopy Finish(
        EndoscopyOutcome outcome,
        DateTimeOffset endedAtUtc,
        string? extentReachedCode = null)
    {
        if (outcome == EndoscopyOutcome.InProgress)
            throw new ArgumentException("A finished endoscopy cannot remain in progress.", nameof(outcome));

        var end = endedAtUtc.ToUniversalTime();
        if (end < StartedAtUtc.ToUniversalTime())
            throw new ArgumentOutOfRangeException(nameof(endedAtUtc), "End time cannot precede start time.");

        return this with
        {
            Outcome = outcome,
            EndedAtUtc = end,
            ExtentReachedCode = string.IsNullOrWhiteSpace(extentReachedCode) ? null : extentReachedCode.Trim()
        };
    }
}

public sealed record EndoscopyFinding(
    Guid Id,
    string AnatomicalSiteCode,
    string Description,
    string? FindingCode = null,
    string? SeverityCode = null,
    decimal? SizeMm = null);

public sealed record EndoscopyTerminationReason(
    Guid Id,
    TerminationReasonKind Kind,
    string? Description = null,
    Guid? RelatedFindingId = null);

public sealed record EndoscopyEvent(
    Guid Id,
    DateTimeOffset OccurredAtUtc,
    EndoscopyPhase Phase,
    string EventCode,
    string? Description = null);
