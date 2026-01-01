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

    [HttpGet("{id:int:min(1)}")]
    public Task<IActionResult> Get(int id, CancellationToken ct)
        => store.GetAsync(new PathologyParcelId(id), ct).ToActionResultAsync(HttpContext);

    [HttpGet("{pathologistId:int:min(1)}/{parcelCode}")]
    public Task<IActionResult> GetByKey(int pathologistId, string parcelCode, CancellationToken ct)
        => store.GetByKeyAsync(new PathologyParcelKeyDto(new PathologistId(pathologistId), parcelCode), ct)
            .ToActionResultAsync(HttpContext);

    // Option 1 (final): Create parcel from endoscopies.
    [HttpPost]
    public Task<IActionResult> Create([FromBody] PathologyParcelCreateRequestDto dto, CancellationToken ct)
        => store.CreateAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpPut("{pathologistId:int:min(1)}/{parcelCode}")]
    public Task<IActionResult> UpdateByKey(int pathologistId, string parcelCode, [FromBody] PathologyParcelUpdateRequestDto dto, CancellationToken ct)
        => store.UpdateByKeyAsync(new PathologyParcelKeyDto(new PathologistId(pathologistId), parcelCode), dto, ct)
            .ToActionResultAsync(HttpContext);

    // Assign many endoscopies (draft parcels only)
    [HttpPost("{pathologistId:int:min(1)}/{parcelCode}/items")]
    public Task<IActionResult> Assign(int pathologistId, string parcelCode, [FromBody] PathologyParcelAssignEndoscopiesRequestDto dto, CancellationToken ct)
        => store.AssignEndoscopiesAsync(new PathologyParcelKeyDto(new PathologistId(pathologistId), parcelCode), dto, ct)
            .ToActionResultAsync(HttpContext);

    // Unassign many endoscopies (draft parcels only)
    [HttpPost("{pathologistId:int:min(1)}/{parcelCode}/items/unassign")]
    public Task<IActionResult> Unassign(int pathologistId, string parcelCode, [FromBody] PathologyParcelAssignEndoscopiesRequestDto dto, CancellationToken ct)
        => store.UnassignEndoscopiesAsync(new PathologyParcelKeyDto(new PathologistId(pathologistId), parcelCode), dto, ct)
            .ToActionResultAsync(HttpContext);

    // Convenience: unassign single
    [HttpDelete("{pathologistId:int:min(1)}/{parcelCode}/items/{endoscopyId:int:min(1)}")]
    public Task<IActionResult> UnassignSingle(int pathologistId, string parcelCode, int endoscopyId, CancellationToken ct)
        => store.UnassignEndoscopiesAsync(
                new PathologyParcelKeyDto(new PathologistId(pathologistId), parcelCode),
                new PathologyParcelAssignEndoscopiesRequestDto([new EndoscopyId(endoscopyId)]),
                ct)
            .ToActionResultAsync(HttpContext);
}
