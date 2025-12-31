using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyParcelAssignReportsRequestDto(
    [param: Required, MinLength(1)] PathologyReportId[] ReportIds);