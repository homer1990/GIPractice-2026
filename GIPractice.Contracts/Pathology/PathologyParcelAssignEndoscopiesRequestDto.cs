using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyParcelAssignEndoscopiesRequestDto(
    [param: Required, MinLength(1)] EndoscopyId[] EndoscopyIds);
