using GIPractice.Contracts.Scheduling;

public sealed class SchedulerOptions
{
    public TimeOnly WorkdayStart { get; init; } = new(8, 0);
    public TimeOnly WorkdayEnd { get; init; } = new(22, 0);
    public int SlotStepMinutes { get; init; } = 30;

    public ClinicCapability DefaultCapabilities { get; init; } =
        ClinicCapability.FrontDeskOpen |
        ClinicCapability.DoctorOnSite |
        ClinicCapability.EndoscopySuiteOpen |
        ClinicCapability.Reporting |
        ClinicCapability.BiopsyHandling;
}
