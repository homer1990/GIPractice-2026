using GIPractice.Core.Enums;

namespace GIPractice.Api.Models;

public class AppointmentCreateDto
{
    public int PatientId { get; set; }

    public AppointmentPurpose Purpose { get; set; }

    public DateTime StartDateTimeUtc { get; set; }
    public DateTime? EndDateTimeUtc { get; set; }

    public bool Urgent { get; set; }

    public EndoscopyType? PlannedEndoscopyType { get; set; }

    public int? PreparationProtocolId { get; set; }

    public string? Notes { get; set; }
}

public class AppointmentDto
{
    public int Id { get; set; }

    public int PatientId { get; set; }
    public string PatientFullName { get; set; } = string.Empty;

    public AppointmentPurpose Purpose { get; set; }

    public DateTime StartDateTimeUtc { get; set; }
    public DateTime? EndDateTimeUtc { get; set; }

    public bool Canceled { get; set; }
    public bool Urgent { get; set; }
    public bool TookPlace { get; set; }

    public EndoscopyType? PlannedEndoscopyType { get; set; }

    public int? PreparationProtocolId { get; set; }
    public string? PreparationProtocolName { get; set; }

    public string? Notes { get; set; }
}
