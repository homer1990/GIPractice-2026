using GIPractice.Core.Entities;
using GIPractice.Core.Enums;

namespace GIPractice.Api.Models;

public class OperationCreateDto
{
    public int PatientId { get; set; }

    public DateTime DateAndTimeUtc { get; set; }

    public OperationTypes Type { get; set; } = OperationTypes.None;

    public Outcomes Outcome { get; set; } = Outcomes.None;

    public int? DoctorId { get; set; }
    public int? FileId { get; set; }
}

public class OperationDto : OperationCreateDto
{
    public int Id { get; set; }

    public string PatientFullName { get; set; } = string.Empty;
    public string? DoctorFullName { get; set; }
}
