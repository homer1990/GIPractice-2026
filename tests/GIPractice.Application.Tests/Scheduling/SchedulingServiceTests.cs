using GIPractice.Application.Scheduling;
using GIPractice.Domain;
using GIPractice.Domain.Encounters;
using GIPractice.Domain.Scheduling;

namespace GIPractice.Application.Tests.Scheduling;

public sealed class SchedulingServiceTests
{
    [Fact]
    public async Task WalkInExclusiveEncounter_ClaimsSingleSlot_WhileInfaiOnlyCanCoexist()
    {
        var patient1 = PatientId.New();
        var patient2 = PatientId.New();
        var store = new FakeSchedulingSessionFactory(patient1, patient2);
        var service = new SchedulingService(store);
        var now = DateTimeOffset.UtcNow;

        var first = await service.StartWalkInEncounterAsync(
            patient1,
            new EncounterPlan(new VisitPlan(VisitKind.Consultation)),
            now);

        await Assert.ThrowsAsync<SchedulingConflictException>(() =>
            service.StartWalkInEncounterAsync(
                patient2,
                new EncounterPlan(new EndoscopyPlan(EndoscopyType.Colonoscopy)),
                now.AddMinutes(1)));

        var infai = await service.StartWalkInEncounterAsync(
            patient2,
            new EncounterPlan(new InfaiPlan()),
            now.AddMinutes(2));

        Assert.Equal(first, store.ActiveEncounterId);
        Assert.NotEqual(first, infai);
    }

    [Fact]
    public async Task CompletingExclusiveEncounter_ReleasesSlotForNextEncounter()
    {
        var patient1 = PatientId.New();
        var patient2 = PatientId.New();
        var store = new FakeSchedulingSessionFactory(patient1, patient2);
        var service = new SchedulingService(store);
        var now = DateTimeOffset.UtcNow;

        var first = await service.StartWalkInEncounterAsync(
            patient1,
            new EncounterPlan(new VisitPlan(VisitKind.Consultation)),
            now);

        await service.CompleteEncounterAsync(first, now.AddMinutes(10));

        var second = await service.StartWalkInEncounterAsync(
            patient2,
            new EncounterPlan(new EndoscopyPlan(EndoscopyType.Gastroscopy)),
            now.AddMinutes(11));

        Assert.Equal(second, store.ActiveEncounterId);
    }

    [Fact]
    public async Task EndoscopyAppointment_CanResolveIntoEndoscopyExamAndPrescriptionInOneEncounter()
    {
        var patient = PatientId.New();
        var store = new FakeSchedulingSessionFactory(patient);
        var service = new SchedulingService(store);
        var now = DateTimeOffset.UtcNow;

        var appointment = await service.ScheduleAppointmentAsync(
            patient,
            AppointmentType.Endoscopy,
            now.AddHours(1),
            durationMinutes: 30,
            isUrgent: true);

        await service.MarkPatientArrivedAsync(appointment);

        var encounter = await service.StartEncounterFromAppointmentAsync(
            appointment,
            new EncounterPlan(
                new EndoscopyPlan(EndoscopyType.Colonoscopy),
                new ClinicalExamPlan(clinicalNotes: "HEINE examination performed during the session"),
                new PrescriptionPlan("Prescription issued after endoscopy")),
            now.AddHours(1).AddMinutes(7));

        Assert.Equal(AppointmentStatus.Resolved, store.GetAppointmentStatus(appointment));
        Assert.Equal(AppointmentType.Endoscopy, store.GetAppointmentType(appointment));
        Assert.Equal(encounter, store.GetLinkedEncounter(appointment));
        Assert.Equal(encounter, store.ActiveEncounterId);

        var details = store.GetEncounterDetails(encounter);
        Assert.Contains(details, detail => detail is Endoscopy);
        Assert.Contains(details, detail => detail is ClinicalExam);
        Assert.Contains(details, detail => detail is Prescription);
    }

    [Fact]
    public async Task WalkIn_CanCombineEndoscopyAndPhysicalExam()
    {
        var patient = PatientId.New();
        var store = new FakeSchedulingSessionFactory(patient);
        var service = new SchedulingService(store);
        var now = DateTimeOffset.UtcNow;

        var encounter = await service.StartWalkInEncounterAsync(
            patient,
            new EncounterPlan(
                new EndoscopyPlan(EndoscopyType.Rectoscopy),
                new ClinicalExamPlan(clinicalNotes: "HEINE examination")),
            now);

        var details = store.GetEncounterDetails(encounter);
        Assert.Equal(2, details.Count);
        Assert.Contains(details, detail => detail is Endoscopy);
        Assert.Contains(details, detail => detail is ClinicalExam);
        Assert.Equal(encounter, store.ActiveEncounterId);
    }

    [Fact]
    public async Task ArrivedAppointment_CanBeMoved_AndKeepsItsOriginalType()
    {
        var patient = PatientId.New();
        var store = new FakeSchedulingSessionFactory(patient);
        var service = new SchedulingService(store);
        var now = DateTimeOffset.UtcNow;

        var appointment = await service.ScheduleAppointmentAsync(
            patient,
            AppointmentType.Endoscopy,
            now,
            durationMinutes: 30);

        await service.MarkPatientArrivedAsync(appointment);

        var movedTo = now.AddHours(1);
        await service.RescheduleAppointmentAsync(appointment, movedTo, 30);

        Assert.Equal(AppointmentStatus.Arrived, store.GetAppointmentStatus(appointment));
        Assert.Equal(AppointmentType.Endoscopy, store.GetAppointmentType(appointment));
        Assert.Equal(movedTo.ToUniversalTime(), store.GetAppointmentStart(appointment));
    }

    [Fact]
    public void EncounterPlan_RejectsDuplicateComponentTypes()
    {
        Assert.Throws<DomainRuleViolationException>(() =>
            new EncounterPlan(
                new ClinicalExamPlan(),
                new ClinicalExamPlan()));
    }
}
