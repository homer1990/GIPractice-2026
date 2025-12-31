using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyParcelSearchRequestDto(
    [param: NonZeroId] PathologistId? PathologistId = null,
    [param: NotDefault] DateTime? CreatedFromUtc = null,
    [param: NotDefault] DateTime? CreatedToUtc = null,
    bool? HasUrgent = null,
    PagedRequestDto? Paging = null);