using System.Net;
using FluentAssertions;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;
using Xunit;

namespace GIPractice.Api.Tests;

public sealed class PathologyReportsApiTests(TestApiFactory factory) : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _http = factory.CreateAuthenticatedClient();
    private readonly TestApiFactory _factory = factory;

    [Fact]
    public async Task Update_ShouldEnforce_ParcelManagedIds_And_ReceiptRules()
    {
        await DevSeedHelper.SeedAsync(_http);

        var (pathologistId, endo1Id, _, _, _) =
            await PathologyTestSeed.SeedPathologistAndTwoEndoscopiesAsync(_factory, 10m, 5m);

        // Create a draft parcel with one endoscopy
        var createParcel = new PathologyParcelCreateRequestDto(
            PathologistId: new PathologistId(pathologistId),
            EndoscopyIds: new[] { new EndoscopyId(endo1Id) },
            DispatchedAtUtc: null,
            CourierName: null,
            TrackingNumber: null,
            Notes: null);

        var pCreateResp = await _http.PostJsonAsync("/api/pathology/parcels", createParcel);
        pCreateResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var pCreateBody = await pCreateResp.Content.ReadJsonAsync<ResultDto<PathologyParcelId>>();
        pCreateBody!.IsSuccess.Should().BeTrue();
        var parcelId = pCreateBody.Value!.Value;

        // Get parcel (needs code + rowversion)
        var pGet = await _http.GetAsync($"/api/pathology/parcels/{parcelId}");
        pGet.StatusCode.Should().Be(HttpStatusCode.OK);
        var pGetBody = await pGet.Content.ReadJsonAsync<ResultDto<PathologyParcelDto>>();
        pGetBody!.IsSuccess.Should().BeTrue();
        var parcel = pGetBody.Value!;

        // Dispatch parcel (sets report SentAtUtc + Dispatched status)
        var dispatchedAt = DateTime.UtcNow;
        var pUpdate = new PathologyParcelUpdateRequestDto(
            Id: parcel.Id,
            DispatchedAtUtc: dispatchedAt,
            CourierName: "Courier",
            TrackingNumber: "TRK",
            Notes: null,
            RowVersion: parcel.RowVersion);

        var keyUrl = $"/api/pathology/parcels/{pathologistId}/{parcel.ParcelCode}";
        var pUpdateResp = await _http.PutJsonAsync(keyUrl, pUpdate);
        pUpdateResp.StatusCode.Should().Be(HttpStatusCode.OK);
        (await pUpdateResp.Content.ReadJsonAsync<ResultDto<bool>>())!.IsSuccess.Should().BeTrue();

        // Find the report created for endo1 by the parcel API
        var search = new PathologyReportSearchRequestDto(
            PatientId: null,
            EndoscopyId: new EndoscopyId(endo1Id),
            PathologistId: new PathologistId(pathologistId),
            DispatchParcelCode: null,
            Status: null,
            IsUrgent: null,
            SentFromUtc: null,
            SentToUtc: null,
            ReceivedFromUtc: null,
            ReceivedToUtc: null,
            Paging: new PagedRequestDto(1, 50));

        var sResp = await _http.PostJsonAsync("/api/pathology/reports/search", search);
        sResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var sBody = await sResp.Content.ReadJsonAsync<ResultDto<PagedResultDto<PathologyReportListItemDto>>>();
        sBody!.IsSuccess.Should().BeTrue();
        var item = sBody.Value!.Items.Single();
        item.SentAtUtc.Should().NotBeNull();
        item.Status.Should().Be(PathologyReportStatus.Dispatched);

        // 1) Cannot set DispatchParcelId via Reports API
        var attemptSetParcel = new PathologyReportUpsertRequestDto(
            Id: item.Id,
            PatientId: item.PatientId,
            EndoscopyId: item.EndoscopyId,
            PathologistId: item.PathologistId,
            DispatchParcelId: new PathologyParcelId(parcelId),
            SentAtUtc: item.SentAtUtc,
            ReceivedAtUtc: null,
            Notes: null,
            ClinicalInfo: null,
            MacroscopyText: null,
            DiagnosisText: null,
            Status: item.Status,
            IsUrgent: item.IsUrgent,
            DocumentFileId: null,
            DocumentKind: PathologyDocumentKind.Unknown,
            DocumentFileName: null,
            DocumentContentType: null,
            Document: null,
            RowVersion: item.RowVersion);

        var bad1 = await _http.PutJsonAsync($"/api/pathology/reports/{item.Id.Value}", attemptSetParcel);
        bad1.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // 2) Cannot set content before receipt
        var attemptContent = attemptSetParcel with
        {
            DispatchParcelId = null,
            DiagnosisText = "diagnosis before receipt",
        };
        var bad2 = await _http.PutJsonAsync($"/api/pathology/reports/{item.Id.Value}", attemptContent);
        bad2.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // 3) Receive: set ReceivedAtUtc + content + status Received
        var receivedAt = item.SentAtUtc!.Value.AddMinutes(5);
        var receive = attemptSetParcel with
        {
            DispatchParcelId = null,
            ReceivedAtUtc = receivedAt,
            DiagnosisText = "Dx",
            MacroscopyText = "Macro",
            Status = PathologyReportStatus.Received,
        };
        var ok = await _http.PutJsonAsync($"/api/pathology/reports/{item.Id.Value}", receive);
        ok.StatusCode.Should().Be(HttpStatusCode.OK);
        (await ok.Content.ReadJsonAsync<ResultDto<bool>>())!.IsSuccess.Should().BeTrue();

        // Get latest rowversion
        var get = await _http.GetAsync($"/api/pathology/reports/{item.Id.Value}");
        get.StatusCode.Should().Be(HttpStatusCode.OK);
        var got = (await get.Content.ReadJsonAsync<ResultDto<PathologyReportDto>>())!.Value!;
        got.ReceivedAtUtc.Should().Be(receivedAt);
        got.DiagnosisText.Should().Be("Dx");

        // 4) Cannot clear ReceivedAtUtc
        var clear = receive with { ReceivedAtUtc = null, RowVersion = got.RowVersion };
        var bad3 = await _http.PutJsonAsync($"/api/pathology/reports/{item.Id.Value}", clear);
        bad3.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // 5) Cannot change EndoscopyId
        var badId = receive with { EndoscopyId = new EndoscopyId(endo1Id + 12345), RowVersion = got.RowVersion };
        var bad4 = await _http.PutJsonAsync($"/api/pathology/reports/{item.Id.Value}", badId);
        bad4.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
