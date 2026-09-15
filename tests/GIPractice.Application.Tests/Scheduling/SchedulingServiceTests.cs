using GIPractice.Application.Scheduling;
using GIPractice.Domain;
using GIPractice.Domain.Encounters;
using GIPractice.Domain.Scheduling;

namespace GIPractice.Application.Tests.Scheduling;

public sealed class SchedulingServiceTests
{
    [Fact]
    public async Task WalkInNonInfaiEncounter_ClaimsTheSingleActiveSlot()
    {
        var patient1 = PatientId.New();
        var patient2 = PatientId.New();
        var store = new FakeSchedulingSessionFactory(patient1, patient2);
        var service = new SchedulingService(store);
        var now = DateTimeOffset.UtcNow;

        var first = await service.StartWalkInEncounterAsync(
            patient1,
            new VisitEncounterPlan(VisitKind.Consultation),
            now);

        await Assert.ThrowsAsync<SchedulingConflictException>(() =>
            service.StartWalkInEncounterAsync(
                patient2,
                new EndoscopyEncounterPlan(EndoscopyType.Colonoscopy),
                now.AddMinutes(1)));

        var infai = await service.StartWalkInEncounterAsync(
            patient2,
            new InfaiEncounterPlan(),
            now.AddMinutes(2));

        Assert.Equal(first, store.ActiveEncounterId);
        Assert.NotEqual(first, infai);
    }

    [Fact]
    public async Task CompletingActiveEncounter_ReleasesSlotForNextEncounter()
    {
        var patient1 = PatientId.New();
        var patient2 = PatientId.New();
        var store = new FakeSchedulingSessionFactory(patient1, patient2);
        var service = new SchedulingService(store);
        var now = DateTimeOffset.UtcNow;

        var first = await service.StartWalkInEncounterAsync(
            patient1,
            new VisitEncounterPlan(VisitKind.Consultation),
            now);

        await service.CompleteEncounterAsync(first, now.AddMinutes(10));

        var second = await service.StartWalkInEncounterAsync(
            patient2,
            new EndoscopyEncounterPlan(EndoscopyType.Gastroscopy),
            now.AddMinutes(11));

        Assert.Equal(second, store.ActiveEncounterId);
    }

    [Fact]
    public async Task AppointmentEncounter_ResolvesAppointmentAndCreatesOneToOneLink()
    {
        var patient = PatientId.New();
        var store = new FakeSchedulingSessionFactory(patient);
        var service = new SchedulingService(store);
        var now = DateTimeOffset.UtcNow;

        var appointment = await service.ScheduleAppointmentAsync(
            patient,
            AppointmentKind.Endoscopy,
            now.AddHours(1),
            durationMinutes: 30,
            isUrgent: true);

        await service.MarkPatientArrivedAsync(appointment);

        var encounter = await service.StartEncounterFromAppointmentAsync(
            appointment,
            new EndoscopyEncounterPlan(EndoscopyType.Colonoscopy),
            now.AddHours(1).AddMinutes(7));

        Assert.Equal(AppointmentStatus.Resolved, store.GetAppointmentStatus(appointment));
        Assert.Equal(encounter, store.GetLinkedEncounter(appointment));
        Assert.Equal(encounter, store.ActiveEncounterId);
    }

    [Fact]
    public async Task AppointmentCannotStartWrongEncounterKind()
    {
        var patient = PatientId.New();
        var store = new FakeSchedulingSessionFactory(patient);
        var service = new SchedulingService(store);
        var now = DateTimeOffset.UtcNow;

        var appointment = await service.ScheduleAppointmentAsync(
            patient,
            AppointmentKind.Visit,
            now,
            durationMinutes: 20);

        await Assert.ThrowsAsync<DomainRuleViolationException>(() =>
            service.StartEncounterFromAppointmentAsync(
                appointment,
                new EndoscopyEncounterPlan(EndoscopyType.Gastroscopy),
                now));
    }

    [Fact]
    public async Task ArrivedAppointment_CannotBeDraggedToAnotherTime()
    {
        var patient = PatientId.New();
        var store = new FakeSchedulingSessionFactory(patient);
        var service = new SchedulingService(store);
        var now = DateTimeOffset.UtcNow;

        var appointment = await service.ScheduleAppointmentAsync(
            patient,
            AppointmentKind.ClinicalExam,
            now,
            durationMinutes: 20);

        await service.MarkPatientArrivedAsync(appointment);

        await Assert.ThrowsAsync<DomainRuleViolationException>(() =>
            service.RescheduleAppointmentAsync(appointment, now.AddHours(1), 20));
    }
}
