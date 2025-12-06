using GIPractice.Core.Abstractions;
using GIPractice.Core.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace GIPractice.Core.Entities;

public class Lab : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public LabType LabTypes { get; set; } = LabType.None;

    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public List<Test> Tests { get; set; } = [];
    public List<Doctor> Doctors { get; set; } = [];
}
