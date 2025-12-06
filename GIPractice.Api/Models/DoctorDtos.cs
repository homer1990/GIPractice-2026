using GIPractice.Core.Enums;

namespace GIPractice.Api.Models;

public class DoctorDto
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    public Specialties Specialty { get; set; } = Specialties.None;

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    public Rating Score { get; set; } = Rating.Default;

    public string Notes { get; set; } = string.Empty;

    public int? LabId { get; set; }
    public string? LabName { get; set; }
}

public class DoctorCreateUpdateDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public Specialties Specialty { get; set; } = Specialties.None;

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    public Rating Score { get; set; } = Rating.Default;

    public string? Notes { get; set; }

    public int? LabId { get; set; }
}
