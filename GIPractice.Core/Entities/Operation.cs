using GIPractice.Core.Abstractions;
using GIPractice.Core.Enums;

namespace GIPractice.Core.Entities;

public enum OperationTypes
{
    None = 0,
    GeneralSurgery = 1,
    EndoscopicTherapy = 2,
    Other = 99
}

public class Operation : BaseEntity
{
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public DateTime DateAndTimeUtc { get; set; }

    public OperationTypes Type { get; set; } = OperationTypes.None;

    public Outcomes Outcome { get; set; } = Outcomes.None;

    public int? FileId { get; set; }
    public MediaFile? File { get; set; }

    public int? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
}
