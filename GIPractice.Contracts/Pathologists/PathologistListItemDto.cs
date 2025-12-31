using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathologists;

public sealed record PathologistListItemDto(
    PathologistId Id,
    string Name,
    string? Email,
    string? PhoneNumber);