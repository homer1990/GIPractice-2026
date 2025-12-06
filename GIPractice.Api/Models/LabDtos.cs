using GIPractice.Core.Enums;

namespace GIPractice.Api.Models;

public class LabDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public LabType LabTypes { get; set; } = LabType.None;

    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}

public class LabCreateUpdateDto
{
    public string Name { get; set; } = string.Empty;

    public LabType LabTypes { get; set; } = LabType.None;

    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}
