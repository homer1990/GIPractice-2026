using GIPractice.Core.Enums;

namespace GIPractice.Api.Models;

public class PatientCreateDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FathersName { get; set; } = string.Empty;

    public string PersonalNumber { get; set; } = string.Empty; // 12-digit string

    public DateTime BirthDay { get; set; }
    public Gender Gender { get; set; }

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
}

public class PatientDto
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FathersName { get; set; } = string.Empty;

    public string PersonalNumber { get; set; } = string.Empty;

    public DateTime BirthDay { get; set; }
    public Gender Gender { get; set; }

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
}
