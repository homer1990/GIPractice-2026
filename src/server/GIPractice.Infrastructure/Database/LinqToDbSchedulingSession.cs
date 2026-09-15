using GIPractice.Application.Scheduling;
using GIPractice.Domain;
using GIPractice.Domain.Encounters;
using GIPractice.Domain.Scheduling;
using LinqToDB;
using LinqToDB.Data;

namespace GIPractice.Infrastructure.Database;

public sealed class LinqToDbSchedulingSessionFactory(DatabaseOptions options) : ISchedulingSessionFactory
{
    private readonly DataOptions _dataOptions = options.CreateLinqToDbOptions();

    public async Task<T> ExecuteAsync<T>(
        Func<ISchedulingSession, CancellationToken, Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        await using var connection = new DataConnection(_dataOptions);
        await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await action(new LinqToDbSchedulingSession(connection), cancellationToken);
            await connection.CommitTransactionAsync(cancellationToken);
            return result;
        }
        catch
        {
            await connection.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

internal sealed class LinqToDbSchedulingSession(DataConnection db) : ISchedulingSession
{
    public Task<bool> PatientExistsAsync(PatientId patientId, CancellationToken cancellationToken) =>
        db.GetTable<PatientRow>()
            .AnyAsync(x => x.Id == patientId.ToString(), cancellationToken);

    public async Task<Appointment?> GetAppointmentAsync(
        AppointmentId appointmentId,
        CancellationToken cancellationToken)
    {
        var row = await db.GetTable<AppointmentRow>()
            .FirstOrDefaultAsync(x => x.Id == appointmentId.ToString(), cancellationToken);

        return row is null ? null : ToDomain(row);
    }

    public async Task InsertAppointmentAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        var affected = await db.InsertAsync(ToRow(appointment), token: cancellationToken);
        EnsureOneRow(affected, "insert appointment");
    }

    public async Task UpdateAppointmentAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        var affected = await db.UpdateAsync(ToRow(appointment), token: cancellationToken);
        EnsureOneRow(affected, "update appointment");
    }

    public async Task<Encounter?> GetEncounterAsync(
        EncounterId encounterId,
        CancellationToken cancellationToken)
    {
        var id = encounterId.ToString();
        var row = await db.GetTable<EncounterRow>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (row is null)
            return null;

        var appointmentId = await db.GetTable<AppointmentEncounterLinkRow>()
            .Where(x => x.EncounterId == id)
            .Select(x => x.AppointmentId)
            .FirstOrDefaultAsync(cancellationToken);

        return ToDomain(row, appointmentId);
    }

    public async Task InsertEncounterAsync(
        Encounter encounter,
        IEncounterDetail detail,
        CancellationToken cancellationToken)
    {
        if (detail.EncounterId != encounter.Id)
            throw new InvalidOperationException("Encounter detail belongs to a different encounter.");

        var affected = await db.InsertAsync(ToRow(encounter), token: cancellationToken);
        EnsureOneRow(affected, "insert encounter");

        affected = detail switch
        {
            Visit visit when encounter.Kind == EncounterKind.Visit =>
                await db.InsertAsync(new VisitRow
                {
                    EncounterId = encounter.Id.ToString(),
                    Kind = (short)visit.Kind
                }, token: cancellationToken),

            Endoscopy endoscopy when encounter.Kind == EncounterKind.Endoscopy =>
                await db.InsertAsync(new EndoscopyRow
                {
                    EncounterId = encounter.Id.ToString(),
                    EndoscopyType = (short)endoscopy.Type,
                    ReportDocumentJson = endoscopy.ReportDocumentJson
                }, token: cancellationToken),

            ClinicalExam exam when encounter.Kind == EncounterKind.ClinicalExam =>
                await db.InsertAsync(new ClinicalExamRow
                {
                    EncounterId = encounter.Id.ToString(),
                    HasSeriousFindings = exam.HasSeriousFindings,
                    ClinicalNotes = exam.ClinicalNotes
                }, token: cancellationToken),

            InfaiTest infai when encounter.Kind == EncounterKind.Infai =>
                await db.InsertAsync(new InfaiTestRow
                {
                    EncounterId = encounter.Id.ToString(),
                    Result = (short)infai.Result,
                    PatientContacted = infai.PatientContacted,
                    ReportStorageKey = infai.ReportStorageKey
                }, token: cancellationToken),

            _ => throw new InvalidOperationException(
                $"Encounter kind {encounter.Kind} does not match detail type {detail.GetType().Name}.")
        };

        EnsureOneRow(affected, "insert encounter detail");
    }

    public async Task UpdateEncounterAsync(Encounter encounter, CancellationToken cancellationToken)
    {
        var affected = await db.UpdateAsync(ToRow(encounter), token: cancellationToken);
        EnsureOneRow(affected, "update encounter");
    }

    public Task<bool> IsAppointmentLinkedAsync(
        AppointmentId appointmentId,
        CancellationToken cancellationToken) =>
        db.GetTable<AppointmentEncounterLinkRow>()
            .AnyAsync(x => x.AppointmentId == appointmentId.ToString(), cancellationToken);

    public async Task LinkAppointmentAsync(
        AppointmentId appointmentId,
        EncounterId encounterId,
        CancellationToken cancellationToken)
    {
        var affected = await db.InsertAsync(new AppointmentEncounterLinkRow
        {
            AppointmentId = appointmentId.ToString(),
            EncounterId = encounterId.ToString()
        }, token: cancellationToken);

        EnsureOneRow(affected, "link appointment to encounter");
    }

    public async Task<bool> TryClaimActiveEncounterAsync(
        EncounterId encounterId,
        CancellationToken cancellationToken)
    {
        var affected = await db.GetTable<PracticeStateRow>()
            .Where(x => x.Id == 1 && x.ActiveEncounterId == null)
            .Set(x => x.ActiveEncounterId, encounterId.ToString())
            .UpdateAsync(cancellationToken);

        return affected == 1;
    }

    public async Task<bool> ReleaseActiveEncounterAsync(
        EncounterId encounterId,
        CancellationToken cancellationToken)
    {
        var id = encounterId.ToString();
        var affected = await db.GetTable<PracticeStateRow>()
            .Where(x => x.Id == 1 && x.ActiveEncounterId == id)
            .Set(x => x.ActiveEncounterId, (string?)null)
            .UpdateAsync(cancellationToken);

        return affected == 1;
    }

    private static AppointmentRow ToRow(Appointment appointment) => new()
    {
        Id = appointment.Id.ToString(),
        PatientId = appointment.PatientId.ToString(),
        Kind = (short)appointment.Kind,
        ScheduledStartUtc = appointment.ScheduledStartUtc.UtcDateTime,
        DurationMinutes = appointment.DurationMinutes,
        Status = (short)appointment.Status,
        IsUrgent = appointment.IsUrgent,
        Notes = appointment.Notes
    };

    private static Appointment ToDomain(AppointmentRow row)
    {
        var appointment = new Appointment(
            new AppointmentId(Guid.Parse(row.Id)),
            new PatientId(Guid.Parse(row.PatientId)),
            (AppointmentKind)row.Kind,
            AsUtc(row.ScheduledStartUtc),
            row.DurationMinutes,
            row.IsUrgent,
            row.Notes);

        switch ((AppointmentStatus)row.Status)
        {
            case AppointmentStatus.Scheduled:
                break;
            case AppointmentStatus.Arrived:
                appointment.MarkArrived();
                break;
            case AppointmentStatus.Resolved:
                appointment.MarkResolved();
                break;
            case AppointmentStatus.Cancelled:
                appointment.Cancel();
                break;
            case AppointmentStatus.NoShow:
                appointment.MarkNoShow();
                break;
            default:
                throw new InvalidDataException($"Unknown appointment status {row.Status}.");
        }

        return appointment;
    }

    private static EncounterRow ToRow(Encounter encounter) => new()
    {
        Id = encounter.Id.ToString(),
        PatientId = encounter.PatientId.ToString(),
        Kind = (short)encounter.Kind,
        StartedAtUtc = encounter.StartedAtUtc.UtcDateTime,
        EndedAtUtc = encounter.EndedAtUtc?.UtcDateTime,
        Status = (short)encounter.Status,
        IsUrgent = encounter.IsUrgent,
        Notes = encounter.Notes
    };

    private static Encounter ToDomain(EncounterRow row, string? appointmentId)
    {
        var encounter = new Encounter(
            new EncounterId(Guid.Parse(row.Id)),
            new PatientId(Guid.Parse(row.PatientId)),
            (EncounterKind)row.Kind,
            AsUtc(row.StartedAtUtc),
            appointmentId is null ? null : new AppointmentId(Guid.Parse(appointmentId)),
            row.IsUrgent,
            row.Notes);

        switch ((EncounterStatus)row.Status)
        {
            case EncounterStatus.Underway:
                break;
            case EncounterStatus.Completed:
                encounter.Complete(AsUtc(row.EndedAtUtc
                    ?? throw new InvalidDataException("Completed encounter has no end time.")));
                break;
            case EncounterStatus.Aborted:
                encounter.Abort(AsUtc(row.EndedAtUtc
                    ?? throw new InvalidDataException("Aborted encounter has no end time.")));
                break;
            default:
                throw new InvalidDataException($"Unknown encounter status {row.Status}.");
        }

        return encounter;
    }

    private static DateTimeOffset AsUtc(DateTime value) =>
        new(DateTime.SpecifyKind(value, DateTimeKind.Utc));

    private static void EnsureOneRow(int affected, string operation)
    {
        if (affected != 1)
            throw new InvalidOperationException($"Database operation '{operation}' affected {affected} rows; expected 1.");
    }
}
