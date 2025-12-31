using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathologists;

public sealed record PathologistUpsertRequestDto(
    [param: NonZeroId] PathologistId? Id,

    [param: Required, MaxLength(200)] string Name,
    [param: MaxLength(500)] string? Address,
    [param: EmailAddress, MaxLength(254)] string? Email,
    [param: MaxLength(50)] string? PhoneNumber,

    // Keep JSON for now; later we can replace with a typed DTO and a converter.
    [param: Required] string PricingPlanJson,

    byte[]? RowVersion);