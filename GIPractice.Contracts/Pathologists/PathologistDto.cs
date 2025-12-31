using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathologists;

public sealed record PathologistDto(
    PathologistId Id,

    string Name,
    string? Address,
    string? Email,
    string? PhoneNumber,

    string PricingPlanJson,

    byte[]? RowVersion);