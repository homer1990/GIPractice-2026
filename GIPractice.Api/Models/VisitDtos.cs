namespace GIPractice.Api.Models;

// GIPractice.Api/Models/Visits/VisitDtos.cs

public class VisitCreateDto
{
    /// <summary>
    /// Appointment that this visit fulfills. Optional for walk-in visits.
    /// </summary>
    public int? AppointmentId { get; set; }

    /// <summary>
    /// Required for walk-in visits (when AppointmentId is null).
    /// Ignored when AppointmentId is provided.
    /// </summary>
    public int? PatientId { get; set; }

    /// <summary>
    /// When the visit actually took place. If null, use now (UTC).
    /// </summary>
    public DateTime? DateOfVisitUtc { get; set; }

    public string? Notes { get; set; }
}


public class VisitDto
{
    public int Id { get; set; }

    public int? AppointmentId { get; set; }

    public int PatientId { get; set; }
    public string PatientFullName { get; set; } = string.Empty;

    public DateTime DateOfVisitUtc { get; set; }
    public string? Notes { get; set; }
}
public class VisitListItemDto
{
    public int Id { get; set; }

    public int PatientId { get; set; }
    public string PatientFullName { get; set; } = string.Empty;

    public DateTime DateOfVisitUtc { get; set; }

    /// <summary>
    /// Short free-text notes from the visit.
    /// </summary>
    public string? Notes { get; set; }
}
public class VisitDetailsDto
{
    public VisitDto Visit { get; set; } = null!;

    /// <summary>
    /// The appointment this visit belongs to, if any.
    /// </summary>
    public AppointmentDto? Appointment { get; set; }

    /// <summary>
    /// All endoscopies performed in this visit.
    /// </summary>
    public List<EndoscopyListItemDto> Endoscopies { get; set; } = [];

    /// <summary>
    /// Diagnoses associated with this visit.
    /// </summary>
    public List<DiagnosisDto> Diagnoses { get; set; } = [];
}