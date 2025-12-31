using GIPractice.Api.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;
using Microsoft.AspNetCore.Mvc;

namespace GIPractice.Api.Pathology;

[ApiController]
[Route("api/pathology/parcels")]
public sealed class PathologyParcelsController(IPathologyParcelsStore store) : ControllerBase
{
    [HttpPost("search")]
    public Task<IActionResult> Search([FromBody] PathologyParcelSearchRequestDto dto, CancellationToken ct)
        => store.SearchAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpGet("{pathologistId:int:min(1)}/{parcelCode}")]
    public Task<IActionResult> GetByKey(int pathologistId, string parcelCode, CancellationToken ct)
        => store.GetByKeyAsync(new PathologyParcelKeyDto(new PathologistId(pathologistId), parcelCode), ct)
            .ToActionResultAsync(HttpContext);

    [HttpPost]
    public Task<IActionResult> Create([FromBody] PathologyParcelCreateRequestDto dto, CancellationToken ct)
        => store.CreateAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpPut("{pathologistId:int:min(1)}/{parcelCode}")]
    public Task<IActionResult> UpdateByKey(int pathologistId, string parcelCode, [FromBody] PathologyParcelUpdateRequestDto dto, CancellationToken ct)
        => store.UpdateByKeyAsync(new PathologyParcelKeyDto(new PathologistId(pathologistId), parcelCode), dto, ct)
            .ToActionResultAsync(HttpContext);

    // Assign many
    [HttpPost("{pathologistId:int:min(1)}/{parcelCode}/items")]
    public Task<IActionResult> Assign(int pathologistId, string parcelCode, [FromBody] PathologyParcelAssignReportsRequestDto dto, CancellationToken ct)
        => store.AssignReportsAsync(new PathologyParcelKeyDto(new PathologistId(pathologistId), parcelCode), dto, ct)
            .ToActionResultAsync(HttpContext);

    // Unassign many (bulk)
    [HttpPost("{pathologistId:int:min(1)}/{parcelCode}/items/unassign")]
    public Task<IActionResult> Unassign(int pathologistId, string parcelCode, [FromBody] PathologyParcelAssignReportsRequestDto dto, CancellationToken ct)
        => store.UnassignReportsAsync(new PathologyParcelKeyDto(new PathologistId(pathologistId), parcelCode), dto, ct)
            .ToActionResultAsync(HttpContext);

    // Convenience: unassign single
    [HttpDelete("{pathologistId:int:min(1)}/{parcelCode}/items/{reportId:int:min(1)}")]
    public Task<IActionResult> UnassignSingle(int pathologistId, string parcelCode, int reportId, CancellationToken ct)
        => store.UnassignReportsAsync(
                new PathologyParcelKeyDto(new PathologistId(pathologistId), parcelCode),
                new PathologyParcelAssignReportsRequestDto([new PathologyReportId(reportId)]),
                ct)
            .ToActionResultAsync(HttpContext);
}
