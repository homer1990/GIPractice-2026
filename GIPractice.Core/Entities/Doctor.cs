using GIPractice.Core.Abstractions;
using GIPractice.Core.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace GIPractice.Core.Entities;

public class Doctor : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public Specialties Specialty { get; set; } = Specialties.None;

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    public Rating Score { get; set; } = Rating.Default;

    public string Notes { get; set; } = string.Empty;

    public int? LabId { get; set; }
    public Lab? Lab { get; set; }

    public List<Test> Tests { get; set; } = [];
    public List<Treatment> Treatments { get; set; } = [];
}
