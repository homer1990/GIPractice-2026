namespace GIPractice.Server.Clinical;

public sealed record Prescription(Guid Id, string? Text = null);

public sealed record Visit(Guid Id, string? Notes = null);

public enum InfaiResult
{
    Pending = 0,
    Negative = 1,
    Positive = 2,
    Indeterminate = 3
}

public sealed record InfaiTest(
    Guid Id,
    InfaiResult Result = InfaiResult.Pending,
    bool PatientContacted = false,
    string? ReportStorageKey = null);
