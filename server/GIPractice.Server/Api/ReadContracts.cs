namespace GIPractice.Server.Api;

public sealed record PatientListItemDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? FathersName,
    DateOnly? BirthDate);

public sealed record PatientDetailsDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? FathersName,
    DateOnly? BirthDate);

public sealed record PatientSearchResponseDto(
    IReadOnlyList<PatientListItemDto> Items,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record AppointmentListItemDto(
    Guid Id,
    Guid PatientId,
    string PatientDisplayName,
    string TypeCode,
    DateTimeOffset StartUtc,
    int DurationMinutes,
    string Status,
    string? Notes);
