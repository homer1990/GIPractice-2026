namespace GIPractice.Server.Clinical;

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

public enum SpecimenPriority
{
    Routine = 0,
    Urgent = 1
}

public sealed record Endoscopy(
    Guid Id,
    string TypeCode,
    DateTimeOffset StartedAtUtc,
    string? Indication = null,
    EndoscopyOutcome Outcome = EndoscopyOutcome.InProgress,
    DateTimeOffset? EndedAtUtc = null,
    string? ExtentReachedCode = null,
    PreparationMode PreparationMode = PreparationMode.NotApplicable,
    string? PreparationQualityCode = null,
    SedationMode SedationMode = SedationMode.None,
    string? Impression = null,
    string? Recommendations = null);

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

public sealed record EndoscopySpecimen(
    Guid Id,
    string AnatomicalSiteCode,
    SpecimenPriority Priority = SpecimenPriority.Routine,
    string? Description = null,
    string? PriorityReason = null,
    Guid? RelatedFindingId = null);
