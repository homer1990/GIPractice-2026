using GIPractice.Domain;
using GIPractice.Domain.Encounters;
using GIPractice.Domain.Scheduling;

namespace GIPractice.Application.Scheduling;

public sealed class SchedulingService(ISchedulingSessionFactory sessions)
{
    public Task<AppointmentId> ScheduleAppointmentAsync(
        PatientId patientId,
        AppointmentKind kind,
        DateTimeOffset scheduledStartUtc,
        int durationMinutes,
        bool isUrgent = false,
        string? notes = null,
        CancellationToken cancellationToken = default) =>
        sessions.ExecuteAsync(async (session, token) =>
        {
            await EnsurePatientExistsAsync(session, patientId, token);

            var appointment = new Appointment(
                AppointmentId.New(),
                patientId,
                kind,
                scheduledStartUtc,
                durationMinutes,
                isUrgent,
                notes);

            await session.InsertAppointmentAsync(appointment, token);
            return appointment.Id;
        }, cancellationToken);

    public Task RescheduleAppointmentAsync(
        AppointmentId appointmentId,
        DateTimeOffset scheduledStartUtc,
        int durationMinutes,
        CancellationToken cancellationToken = default) =>
        sessions.ExecuteAsync(async (session, token) =>
        {
            var appointment = await RequireAppointmentAsync(session, appointmentId, token);
            appointment.Reschedule(scheduledStartUtc, durationMinutes);
            await session.UpdateAppointmentAsync(appointment, token);
            return true;
        }, cancellationToken);

    public Task MarkPatientArrivedAsync(
        AppointmentId appointmentId,
        CancellationToken cancellationToken = default) =>
        sessions.ExecuteAsync(async (session, token) =>
        {
            var appointment = await RequireAppointmentAsync(session, appointmentId, token);
            appointment.MarkArrived();
            await session.UpdateAppointmentAsync(appointment, token);
            return true;
        }, cancellationToken);

    public Task CancelAppointmentAsync(
        AppointmentId appointmentId,
        CancellationToken cancellationToken = default) =>
        sessions.ExecuteAsync(async (session, token) =>
        {
            var appointment = await RequireAppointmentAsync(session, appointmentId, token);
            appointment.Cancel();
            await session.UpdateAppointmentAsync(appointment, token);
            return true;
        }, cancellationToken);

    public Task<EncounterId> StartEncounterFromAppointmentAsync(
        AppointmentId appointmentId,
        EncounterPlan plan,
        DateTimeOffset startedAtUtc,
        string? notes = null,
        CancellationToken cancellationToken = default) =>
        sessions.ExecuteAsync(async (session, token) =>
        {
            ArgumentNullException.ThrowIfNull(plan);

            var appointment = await RequireAppointmentAsync(session, appointmentId, token);
            if (appointment.Status is not (AppointmentStatus.Scheduled or AppointmentStatus.Arrived))
                throw new DomainRuleViolationException($"Appointment cannot start an encounter from {appointment.Status}.");

            if (await session.IsAppointmentLinkedAsync(appointment.Id, token))
                throw new SchedulingConflictException("Appointment already has an encounter.");

            var expectedKind = ToEncounterKind(appointment.Kind);
            if (plan.Kind != expectedKind)
                throw new DomainRuleViolationException(
                    $"Appointment kind {appointment.Kind} cannot create encounter kind {plan.Kind}.");

            var encounter = new Encounter(
                EncounterId.New(),
                appointment.PatientId,
                plan.Kind,
                startedAtUtc,
                appointment.Id,
                appointment.IsUrgent,
                notes);

            await PersistNewEncounterAsync(session, encounter, plan, token);
            await session.LinkAppointmentAsync(appointment.Id, encounter.Id, token);

            appointment.MarkResolved();
            await session.UpdateAppointmentAsync(appointment, token);

            return encounter.Id;
        }, cancellationToken);

    public Task<EncounterId> StartWalkInEncounterAsync(
        PatientId patientId,
        EncounterPlan plan,
        DateTimeOffset startedAtUtc,
        bool isUrgent = false,
        string? notes = null,
        CancellationToken cancellationToken = default) =>
        sessions.ExecuteAsync(async (session, token) =>
        {
            ArgumentNullException.ThrowIfNull(plan);
            await EnsurePatientExistsAsync(session, patientId, token);

            var encounter = new Encounter(
                EncounterId.New(),
                patientId,
                plan.Kind,
                startedAtUtc,
                appointmentId: null,
                isUrgent,
                notes);

            await PersistNewEncounterAsync(session, encounter, plan, token);
            return encounter.Id;
        }, cancellationToken);

    public Task CompleteEncounterAsync(
        EncounterId encounterId,
        DateTimeOffset endedAtUtc,
        CancellationToken cancellationToken = default) =>
        EndEncounterAsync(encounterId, endedAtUtc, abort: false, cancellationToken);

    public Task AbortEncounterAsync(
        EncounterId encounterId,
        DateTimeOffset endedAtUtc,
        CancellationToken cancellationToken = default) =>
        EndEncounterAsync(encounterId, endedAtUtc, abort: true, cancellationToken);

    private Task EndEncounterAsync(
        EncounterId encounterId,
        DateTimeOffset endedAtUtc,
        bool abort,
        CancellationToken cancellationToken) =>
        sessions.ExecuteAsync(async (session, token) =>
        {
            var encounter = await session.GetEncounterAsync(encounterId, token)
                ?? throw new EntityNotFoundException($"Encounter {encounterId} was not found.");

            var occupiedActiveSlot = encounter.OccupiesActiveSlot;
            if (abort)
                encounter.Abort(endedAtUtc);
            else
                encounter.Complete(endedAtUtc);

            await session.UpdateEncounterAsync(encounter, token);

            if (occupiedActiveSlot && !await session.ReleaseActiveEncounterAsync(encounter.Id, token))
                throw new SchedulingStateException(
                    $"Encounter {encounter.Id} was underway but did not own the practice active slot.");

            return true;
        }, cancellationToken);

    private static async Task PersistNewEncounterAsync(
        ISchedulingSession session,
        Encounter encounter,
        EncounterPlan plan,
        CancellationToken cancellationToken)
    {
        var detail = plan.CreateDetail(encounter.Id);
        await session.InsertEncounterAsync(encounter, detail, cancellationToken);

        if (encounter.OccupiesActiveSlot &&
            !await session.TryClaimActiveEncounterAsync(encounter.Id, cancellationToken))
        {
            throw new SchedulingConflictException(
                "Another non-INFAI encounter is already underway. Complete or abort it before starting this encounter.");
        }
    }

    private static async Task EnsurePatientExistsAsync(
        ISchedulingSession session,
        PatientId patientId,
        CancellationToken cancellationToken)
    {
        if (!await session.PatientExistsAsync(patientId, cancellationToken))
            throw new EntityNotFoundException($"Patient {patientId} was not found.");
    }

    private static async Task<Appointment> RequireAppointmentAsync(
        ISchedulingSession session,
        AppointmentId appointmentId,
        CancellationToken cancellationToken) =>
        await session.GetAppointmentAsync(appointmentId, cancellationToken)
        ?? throw new EntityNotFoundException($"Appointment {appointmentId} was not found.");

    private static EncounterKind ToEncounterKind(AppointmentKind kind) => kind switch
    {
        AppointmentKind.Visit => EncounterKind.Visit,
        AppointmentKind.Endoscopy => EncounterKind.Endoscopy,
        AppointmentKind.ClinicalExam => EncounterKind.ClinicalExam,
        AppointmentKind.Infai => EncounterKind.Infai,
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
    };
}
