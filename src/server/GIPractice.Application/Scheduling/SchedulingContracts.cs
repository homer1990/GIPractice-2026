using GIPractice.Domain;
using GIPractice.Domain.Encounters;
using GIPractice.Domain.Scheduling;

namespace GIPractice.Application.Scheduling;

public interface ISchedulingSessionFactory
{
    Task<T> ExecuteAsync<T>(
        Func<ISchedulingSession, CancellationToken, Task<T>> action,
        CancellationToken cancellationToken = default);
}

public interface ISchedulingSession
{
    Task<bool> PatientExistsAsync(PatientId patientId, CancellationToken cancellationToken);

    Task<Appointment?> GetAppointmentAsync(AppointmentId appointmentId, CancellationToken cancellationToken);
    Task InsertAppointmentAsync(Appointment appointment, CancellationToken cancellationToken);
    Task UpdateAppointmentAsync(Appointment appointment, CancellationToken cancellationToken);

    Task<Encounter?> GetEncounterAsync(EncounterId encounterId, CancellationToken cancellationToken);
    Task InsertEncounterAsync(
        Encounter encounter,
        IReadOnlyCollection<IEncounterDetail> details,
        CancellationToken cancellationToken);
    Task UpdateEncounterAsync(Encounter encounter, CancellationToken cancellationToken);

    Task<bool> IsAppointmentLinkedAsync(AppointmentId appointmentId, CancellationToken cancellationToken);
    Task LinkAppointmentAsync(AppointmentId appointmentId, EncounterId encounterId, CancellationToken cancellationToken);

    Task<bool> TryClaimActiveEncounterAsync(EncounterId encounterId, CancellationToken cancellationToken);
    Task<bool> ReleaseActiveEncounterAsync(EncounterId encounterId, CancellationToken cancellationToken);
}

public sealed class SchedulingConflictException(string message) : InvalidOperationException(message);
public sealed class SchedulingStateException(string message) : InvalidOperationException(message);
public sealed class EntityNotFoundException(string message) : InvalidOperationException(message);
