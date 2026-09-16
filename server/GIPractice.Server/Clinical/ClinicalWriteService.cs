namespace GIPractice.Server.Clinical;

// Specific persistence operations for clinical writes. This is intentionally not a generic repository.
internal interface IClinicalWriteStore
{
    Task<Guid> CreateSessionAsync(
        Guid patientId,
        Guid? appointmentId,
        DateTimeOffset startedAtUtc,
        CancellationToken cancellationToken);

    Task AddEndoscopyAsync(Guid sessionId, Endoscopy endoscopy, CancellationToken cancellationToken);
    Task<Endoscopy?> GetEndoscopyAsync(Guid endoscopyId, CancellationToken cancellationToken);
    Task UpdateEndoscopyAsync(Endoscopy endoscopy, CancellationToken cancellationToken);
    Task AddEndoscopyFindingAsync(Guid endoscopyId, EndoscopyFinding finding, CancellationToken cancellationToken);
    Task AddEndoscopyTerminationReasonAsync(Guid endoscopyId, EndoscopyTerminationReason reason, CancellationToken cancellationToken);
    Task AddEndoscopyEventAsync(Guid endoscopyId, EndoscopyEvent timelineEvent, CancellationToken cancellationToken);
    Task AddEndoscopySpecimenAsync(Guid endoscopyId, EndoscopySpecimen specimen, CancellationToken cancellationToken);

    Task AddExamAsync(Guid sessionId, ClinicalExam exam, CancellationToken cancellationToken);
    Task AddPrescriptionAsync(Guid sessionId, Prescription prescription, CancellationToken cancellationToken);
    Task AddVisitAsync(Guid sessionId, Visit visit, CancellationToken cancellationToken);
    Task AddInfaiAsync(Guid sessionId, InfaiTest infai, CancellationToken cancellationToken);
}

// Internal handle only. It may be carried by the API/client service while a workflow is open,
// but it is not a user-facing domain object and must not be exposed to QML as an Encounter.
internal readonly record struct ClinicalSessionKey(Guid Value);

internal sealed record StartedClinicalItem<T>(T Item, ClinicalSessionKey Session);

internal sealed class ClinicalWriteService(IClinicalWriteStore store)
{
    public async Task<StartedClinicalItem<Endoscopy>> StartEndoscopyAsync(
        Guid patientId,
        Guid? appointmentId,
        string typeCode,
        DateTimeOffset startedAtUtc,
        string? indication = null,
        ProcedurePriority priority = ProcedurePriority.Routine,
        CancellationToken cancellationToken = default)
    {
        var session = await CreateSessionAsync(patientId, appointmentId, startedAtUtc, cancellationToken);
        var item = new Endoscopy(
            Guid.CreateVersion7(),
            RequiredCode(typeCode),
            startedAtUtc.ToUniversalTime(),
            NormalizeText(indication),
            priority);

        await store.AddEndoscopyAsync(session.Value, item, cancellationToken);
        return new(item, session);
    }

    public async Task<EndoscopyFinding> AddEndoscopyFindingAsync(
        Guid endoscopyId,
        string anatomicalSiteCode,
        string description,
        string? findingCode = null,
        string? severityCode = null,
        decimal? sizeMm = null,
        CancellationToken cancellationToken = default)
    {
        RequireId(endoscopyId, nameof(endoscopyId));
        if (sizeMm is < 0) throw new ArgumentOutOfRangeException(nameof(sizeMm));

        var finding = new EndoscopyFinding(
            Guid.CreateVersion7(),
            RequiredCode(anatomicalSiteCode),
            RequiredText(description),
            OptionalCode(findingCode),
            OptionalCode(severityCode),
            sizeMm);

        await store.AddEndoscopyFindingAsync(endoscopyId, finding, cancellationToken);
        return finding;
    }

    public async Task<EndoscopyTerminationReason> AddEndoscopyTerminationReasonAsync(
        Guid endoscopyId,
        TerminationReasonKind kind,
        string? description = null,
        Guid? relatedFindingId = null,
        CancellationToken cancellationToken = default)
    {
        RequireId(endoscopyId, nameof(endoscopyId));
        if (relatedFindingId == Guid.Empty) relatedFindingId = null;

        var reason = new EndoscopyTerminationReason(
            Guid.CreateVersion7(),
            kind,
            NormalizeText(description),
            relatedFindingId);

        await store.AddEndoscopyTerminationReasonAsync(endoscopyId, reason, cancellationToken);
        return reason;
    }

    public async Task<EndoscopyEvent> AddEndoscopyEventAsync(
        Guid endoscopyId,
        DateTimeOffset occurredAtUtc,
        EndoscopyPhase phase,
        string eventCode,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        RequireId(endoscopyId, nameof(endoscopyId));

        var timelineEvent = new EndoscopyEvent(
            Guid.CreateVersion7(),
            occurredAtUtc.ToUniversalTime(),
            phase,
            RequiredCode(eventCode),
            NormalizeText(description));

        await store.AddEndoscopyEventAsync(endoscopyId, timelineEvent, cancellationToken);
        return timelineEvent;
    }

    public async Task<EndoscopySpecimen> AddEndoscopySpecimenAsync(
        Guid endoscopyId,
        string anatomicalSiteCode,
        SpecimenPriority priority = SpecimenPriority.Routine,
        string? description = null,
        string? priorityReason = null,
        Guid? relatedFindingId = null,
        CancellationToken cancellationToken = default)
    {
        RequireId(endoscopyId, nameof(endoscopyId));
        if (relatedFindingId == Guid.Empty) relatedFindingId = null;

        var specimen = new EndoscopySpecimen(
            Guid.CreateVersion7(),
            RequiredCode(anatomicalSiteCode),
            priority,
            NormalizeText(description),
            NormalizeText(priorityReason),
            relatedFindingId);

        await store.AddEndoscopySpecimenAsync(endoscopyId, specimen, cancellationToken);
        return specimen;
    }

    public async Task<Endoscopy> FinishEndoscopyAsync(
        Guid endoscopyId,
        EndoscopyOutcome outcome,
        DateTimeOffset endedAtUtc,
        string? extentReachedCode = null,
        CancellationToken cancellationToken = default)
    {
        RequireId(endoscopyId, nameof(endoscopyId));

        var current = await store.GetEndoscopyAsync(endoscopyId, cancellationToken)
            ?? throw new InvalidOperationException($"Endoscopy {endoscopyId} was not found.");

        if (current.Outcome != EndoscopyOutcome.InProgress)
            throw new InvalidOperationException($"Endoscopy {endoscopyId} is already {current.Outcome}.");

        var finished = current.Finish(outcome, endedAtUtc, OptionalCode(extentReachedCode));
        await store.UpdateEndoscopyAsync(finished, cancellationToken);
        return finished;
    }

    public async Task<StartedClinicalItem<ClinicalExam>> StartExamAsync(
        Guid patientId,
        Guid? appointmentId,
        DateTimeOffset startedAtUtc,
        string? examinationText = null,
        string? assessment = null,
        CancellationToken cancellationToken = default)
    {
        var session = await CreateSessionAsync(patientId, appointmentId, startedAtUtc, cancellationToken);
        var item = ClinicalExam.Create(examinationText, assessment);
        await store.AddExamAsync(session.Value, item, cancellationToken);
        return new(item, session);
    }

    public async Task<ClinicalExam> AddExamAsync(
        ClinicalSessionKey session,
        string? examinationText = null,
        string? assessment = null,
        CancellationToken cancellationToken = default)
    {
        var item = ClinicalExam.Create(examinationText, assessment);
        await store.AddExamAsync(session.Value, item, cancellationToken);
        return item;
    }

    public async Task<Prescription> AddPrescriptionAsync(
        ClinicalSessionKey session,
        string? text,
        CancellationToken cancellationToken = default)
    {
        var item = new Prescription(Guid.CreateVersion7(), NormalizeText(text));
        await store.AddPrescriptionAsync(session.Value, item, cancellationToken);
        return item;
    }

    public async Task<Visit> AddVisitAsync(
        ClinicalSessionKey session,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        var item = new Visit(Guid.CreateVersion7(), NormalizeText(notes));
        await store.AddVisitAsync(session.Value, item, cancellationToken);
        return item;
    }

    public async Task<StartedClinicalItem<InfaiTest>> StartInfaiAsync(
        Guid patientId,
        Guid? appointmentId,
        DateTimeOffset startedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var session = await CreateSessionAsync(patientId, appointmentId, startedAtUtc, cancellationToken);
        var item = new InfaiTest(Guid.CreateVersion7());
        await store.AddInfaiAsync(session.Value, item, cancellationToken);
        return new(item, session);
    }

    private async Task<ClinicalSessionKey> CreateSessionAsync(
        Guid patientId,
        Guid? appointmentId,
        DateTimeOffset startedAtUtc,
        CancellationToken cancellationToken)
    {
        RequireId(patientId, nameof(patientId));
        if (appointmentId == Guid.Empty) appointmentId = null;

        var id = await store.CreateSessionAsync(
            patientId,
            appointmentId,
            startedAtUtc.ToUniversalTime(),
            cancellationToken);

        if (id == Guid.Empty) throw new InvalidOperationException("Clinical store returned an empty session ID.");
        return new(id);
    }

    private static void RequireId(Guid value, string parameterName)
    {
        if (value == Guid.Empty) throw new ArgumentException("ID is required.", parameterName);
    }

    private static string RequiredCode(string value)
    {
        var normalized = value.Trim();
        if (normalized.Length == 0) throw new ArgumentException("Code is required.", nameof(value));
        if (normalized.Length > 128) throw new ArgumentOutOfRangeException(nameof(value));
        return normalized;
    }

    private static string? OptionalCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = value.Trim();
        if (normalized.Length > 128) throw new ArgumentOutOfRangeException(nameof(value));
        return normalized;
    }

    private static string RequiredText(string value)
    {
        var normalized = value.Trim();
        if (normalized.Length == 0) throw new ArgumentException("Text is required.", nameof(value));
        return normalized;
    }

    private static string? NormalizeText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return value.Trim();
    }
}
