namespace GIPractice.Contracts.Scheduling;

[Flags]
public enum ClinicCapability
{
    None = 0,

    // People / workflow
    FrontDeskOpen = 1 << 0,  // secretary / scheduling / phone
    DoctorOnSite = 1 << 1,

    // Rooms / modalities
    EndoscopySuiteOpen = 1 << 2,
    OrthoHeineOpen = 1 << 3,

    // Back-office / pipeline
    BiopsyHandling = 1 << 4,  // labeling, bundling, dispatch prep
    Reporting = 1 << 5,  // writing reports, editing records
}

public sealed record CalendarDayMetaDto(
    DateOnly Day,
    bool IsHoliday,
    bool IsDayOff,
    string? Notes,
    ClinicCapability Capabilities,
    byte[]? RowVersion);