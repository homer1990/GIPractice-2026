using GIPractice.Core.Abstractions;
using GIPractice.Core.Enums;
using GIPractice.Core.ValueObjects;

namespace GIPractice.Core.Entities;

public class Patient : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FathersName { get; set; } = string.Empty;

    /// <summary>Unified Personal Identification Number (Προσωπικός Αριθμός).</summary>
    public PersonalNumber PersonalNumber { get; set; }

    public DateTime BirthDay { get; set; }
    public Gender Gender { get; set; } = Gender.None;

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    public List<Appointment> Appointments { get; set; } = [];
    public List<Encounter> Encounters { get; set; } = [];

    public List<Diagnosis> Diagnoses { get; set; } = [];
    public List<Test> Tests { get; set; } = [];
    public List<Treatment> Treatments { get; set; } = [];
    public List<Operation> Operations { get; set; } = [];
    public List<Report> BiopsyReports { get; set; } = [];
    public List<BiopsyBottle> BiopsyBottles { get; set; } = [];
}
