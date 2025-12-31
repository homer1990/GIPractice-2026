using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyParcelKeyDto(
    [param: NonZeroId] PathologistId PathologistId,
    [param: Required, MaxLength(32)] string ParcelCode);