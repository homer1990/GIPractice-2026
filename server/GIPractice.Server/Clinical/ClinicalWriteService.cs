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
        CancellationToken cancellationToken = default)
    {
        var session = await CreateSessionAsync(patientId, appointmentId, startedAtUtc, cancellationToken);
        var item = new Endoscopy(Guid.CreateVersion7(), RequiredCode(typeCode));
        await store.AddEndoscopyAsync(session.Value, item, cancellationToken);
        return new(item, session);
    }

    public async Task<StartedClinicalItem<ClinicalExam>> StartExamAsync(
        Guid patientId,
        Guid? appointmentId,
        string typeCode,
        DateTimeOffset startedAtUtc,
        string? findings = null,
        CancellationToken cancellationToken = default)
    {
        var session = await CreateSessionAsync(patientId, appointmentId, startedAtUtc, cancellationToken);
        var item = new ClinicalExam(Guid.CreateVersion7(), RequiredCode(typeCode), NormalizeText(findings));
        await store.AddExamAsync(session.Value, item, cancellationToken);
        return new(item, session);
    }

    public async Task<ClinicalExam> AddExamAsync(
        ClinicalSessionKey session,
        string typeCode,
        string? findings = null,
        CancellationToken cancellationToken = default)
    {
        var item = new ClinicalExam(Guid.CreateVersion7(), RequiredCode(typeCode), NormalizeText(findings));
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
        if (patientId == Guid.Empty) throw new ArgumentException("Patient ID is required.", nameof(patientId));
        if (appointmentId == Guid.Empty) appointmentId = null;

        var id = await store.CreateSessionAsync(
            patientId,
            appointmentId,
            startedAtUtc.ToUniversalTime(),
            cancellationToken);

        if (id == Guid.Empty) throw new InvalidOperationException("Clinical store returned an empty session ID.");
        return new(id);
    }

    private static string RequiredCode(string value)
    {
        var normalized = value.Trim();
        if (normalized.Length == 0) throw new ArgumentException("Type code is required.", nameof(value));
        if (normalized.Length > 64) throw new ArgumentOutOfRangeException(nameof(value));
        return normalized;
    }

    private static string? NormalizeText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return value.Trim();
    }
}
