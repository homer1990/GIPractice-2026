using GIPractice.Core.Enums;

namespace GIPractice.Api.Models;

public class TreatmentCreateDto
{
    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public DateTime StartUtc { get; set; }

    public DateTime? EndUtc { get; set; }

    public Outcomes Outcome { get; set; } = Outcomes.None;

    public bool IsChronic { get; set; }

    public string? Notes { get; set; }

    // IDs of linked diagnoses and medicines (optional; adapt to your needs)
    public List<int> DiagnosisIds { get; set; } = [];
    public List<int> MedicineIds { get; set; } = [];
}

public class TreatmentDto : TreatmentCreateDto
{
    public int Id { get; set; }

    public string PatientFullName { get; set; } = string.Empty;

    public string DoctorFullName { get; set; } = string.Empty;
}
