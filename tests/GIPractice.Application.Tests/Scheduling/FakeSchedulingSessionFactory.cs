using GIPractice.Application.Scheduling;
using GIPractice.Domain;
using GIPractice.Domain.Encounters;
using GIPractice.Domain.Scheduling;

namespace GIPractice.Application.Tests.Scheduling;

internal sealed class FakeSchedulingSessionFactory : ISchedulingSessionFactory
{
    private State _state;

    public FakeSchedulingSessionFactory(params PatientId[] patients)
    {
        _state = new State();
        foreach (var patient in patients)
            _state.Patients.Add(patient);
    }

    public EncounterId? ActiveEncounterId => _state.ActiveEncounterId;

    public AppointmentStatus GetAppointmentStatus(AppointmentId appointmentId) =>
        _state.Appointments[appointmentId].Status;

    public EncounterId? GetLinkedEncounter(AppointmentId appointmentId) =>
        _state.Links.TryGetValue(appointmentId, out var encounterId) ? encounterId : null;

    public async Task<T> ExecuteAsync<T>(
        Func<ISchedulingSession, CancellationToken, Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        var working = _state.Clone();
        var result = await action(new FakeSchedulingSession(working), cancellationToken);
        _state = working;
        return result;
    }

    private sealed class State
    {
        public HashSet<PatientId> Patients { get; } = [];
        public Dictionary<AppointmentId, Appointment> Appointments { get; } = [];
        public Dictionary<EncounterId, Encounter> Encounters { get; } = [];
        public Dictionary<EncounterId, IEncounterDetail> Details { get; } = [];
        public Dictionary<AppointmentId, EncounterId> Links { get; } = [];
        public EncounterId? ActiveEncounterId { get; set; }

        public State Clone()
        {
            var clone = new State { ActiveEncounterId = ActiveEncounterId };
            clone.Patients.UnionWith(Patients);

            foreach (var pair in Appointments)
                clone.Appointments.Add(pair.Key, CloneAppointment(pair.Value));

            foreach (var pair in Encounters)
                clone.Encounters.Add(pair.Key, CloneEncounter(pair.Value));

            foreach (var pair in Details)
                clone.Details.Add(pair.Key, pair.Value);

            foreach (var pair in Links)
                clone.Links.Add(pair.Key, pair.Value);

            return clone;
        }

        private static Appointment CloneAppointment(Appointment source)
        {
            var clone = new Appointment(
                source.Id,
                source.PatientId,
                source.Kind,
                source.ScheduledStartUtc,
                source.DurationMinutes,
                source.IsUrgent,
                source.Notes);

            switch (source.Status)
            {
                case AppointmentStatus.Scheduled:
                    break;
                case AppointmentStatus.Arrived:
                    clone.MarkArrived();
                    break;
                case AppointmentStatus.Resolved:
                    clone.MarkResolved();
                    break;
                case AppointmentStatus.Cancelled:
                    clone.Cancel();
                    break;
                case AppointmentStatus.NoShow:
                    clone.MarkNoShow();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source));
            }

            return clone;
        }

        private static Encounter CloneEncounter(Encounter source)
        {
            var clone = new Encounter(
                source.Id,
                source.PatientId,
                source.Kind,
                source.StartedAtUtc,
                source.AppointmentId,
                source.IsUrgent,
                source.Notes);

            switch (source.Status)
            {
                case EncounterStatus.Underway:
                    break;
                case EncounterStatus.Completed:
                    clone.Complete(source.EndedAtUtc ?? throw new InvalidOperationException());
                    break;
                case EncounterStatus.Aborted:
                    clone.Abort(source.EndedAtUtc ?? throw new InvalidOperationException());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source));
            }

            return clone;
        }
    }

    private sealed class FakeSchedulingSession(State state) : ISchedulingSession
    {
        public Task<bool> PatientExistsAsync(PatientId patientId, CancellationToken cancellationToken) =>
            Task.FromResult(state.Patients.Contains(patientId));

        public Task<Appointment?> GetAppointmentAsync(AppointmentId appointmentId, CancellationToken cancellationToken) =>
            Task.FromResult(state.Appointments.GetValueOrDefault(appointmentId));

        public Task InsertAppointmentAsync(Appointment appointment, CancellationToken cancellationToken)
        {
            state.Appointments.Add(appointment.Id, appointment);
            return Task.CompletedTask;
        }

        public Task UpdateAppointmentAsync(Appointment appointment, CancellationToken cancellationToken)
        {
            state.Appointments[appointment.Id] = appointment;
            return Task.CompletedTask;
        }

        public Task<Encounter?> GetEncounterAsync(EncounterId encounterId, CancellationToken cancellationToken) =>
            Task.FromResult(state.Encounters.GetValueOrDefault(encounterId));

        public Task InsertEncounterAsync(
            Encounter encounter,
            IEncounterDetail detail,
            CancellationToken cancellationToken)
        {
            state.Encounters.Add(encounter.Id, encounter);
            state.Details.Add(encounter.Id, detail);
            return Task.CompletedTask;
        }

        public Task UpdateEncounterAsync(Encounter encounter, CancellationToken cancellationToken)
        {
            state.Encounters[encounter.Id] = encounter;
            return Task.CompletedTask;
        }

        public Task<bool> IsAppointmentLinkedAsync(
            AppointmentId appointmentId,
            CancellationToken cancellationToken) =>
            Task.FromResult(state.Links.ContainsKey(appointmentId));

        public Task LinkAppointmentAsync(
            AppointmentId appointmentId,
            EncounterId encounterId,
            CancellationToken cancellationToken)
        {
            state.Links.Add(appointmentId, encounterId);
            return Task.CompletedTask;
        }

        public Task<bool> TryClaimActiveEncounterAsync(
            EncounterId encounterId,
            CancellationToken cancellationToken)
        {
            if (state.ActiveEncounterId is not null)
                return Task.FromResult(false);

            state.ActiveEncounterId = encounterId;
            return Task.FromResult(true);
        }

        public Task<bool> ReleaseActiveEncounterAsync(
            EncounterId encounterId,
            CancellationToken cancellationToken)
        {
            if (state.ActiveEncounterId != encounterId)
                return Task.FromResult(false);

            state.ActiveEncounterId = null;
            return Task.FromResult(true);
        }
    }
}
