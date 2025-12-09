using System;

namespace GIPractice.Api.Models;

public class PatientSearchRequestDto
{
    public string? PersonalNumber { get; set; }
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public string? FathersName { get; set; }
    public DateTime? BirthDateFrom { get; set; }
    public DateTime? BirthDateTo { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    // paging
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    // sorting
    /// <summary>
    /// Allowed values (case-insensitive): "LastVisit", "LastName", "FirstName", "BirthDay", "PersonalNumber", "Id".
    /// </summary>
    public string? SortField { get; set; }

    /// <summary>
    /// True = descending, False = ascending. Default is true for LastVisit.
    /// </summary>
    public bool SortDescending { get; set; } = true;
}


public class PatientListItemDto
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FathersName { get; set; } = string.Empty;

    public string FullName => $"{LastName} {FirstName}";

    public string PersonalNumber { get; set; } = string.Empty;

    public DateTime BirthDay { get; set; }
    public int AgeYears { get; set; }

    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public DateTime? LastVisitUtc { get; set; }
}
