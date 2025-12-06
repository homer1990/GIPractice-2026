using GIPractice.Core.Entities;
using GIPractice.Core.Enums;

namespace GIPractice.Api.Models;

/// <summary>
/// Lightweight list item for Operations.
/// </summary>
public class OperationListItemDto
{
    public int Id { get; set; }

    public int PatientId { get; set; }
    public string PatientFullName { get; set; } = string.Empty;

    public DateTime DateAndTimeUtc { get; set; }

    public OperationTypes Type { get; set; }
    public Outcomes Outcome { get; set; }
}

/// <summary>
/// Lightweight list item for lab Tests.
/// </summary>
public class TestListItemDto
{
    public int Id { get; set; }

    public int PatientId { get; set; }
    public string PatientFullName { get; set; } = string.Empty;

    public DateTime PerformedAtUtc { get; set; }

    public TestType TestType { get; set; }
    public QualitativeTestResults QualitativeResult { get; set; }

    /// <summary>
    /// True if a numeric result is present (we don't care about the actual numeric type here).
    /// </summary>
    public bool HasQuantitativeResult { get; set; }

    public string? Notes { get; set; }
}

/// <summary>
/// Lightweight list item for Infai breath tests.
/// </summary>
public class InfaiTestListItemDto
{
    public int Id { get; set; }

    public int PatientId { get; set; }
    public string PatientFullName { get; set; } = string.Empty;

    public DateTime PerformedAtUtc { get; set; }

    /// <summary>
    /// User-friendly text representation of the result (enum/string etc.).
    /// </summary>
    public string ResultDisplay { get; set; } = string.Empty;

    public string? Notes { get; set; }
}
