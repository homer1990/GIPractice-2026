using System.Net;
using FluentAssertions;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;
using Xunit;

namespace GIPractice.Api.Tests;

public sealed class PathologyReportsApiTests(TestApiFactory factory) : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _http = factory.CreateClient();
    private readonly TestApiFactory _factory = factory;

    [Fact]
    public async Task UpdateReport_CannotChangeDispatchParcelId_ViaReportsEndpoint()
    {
        await DevSeedHelper.SeedAsync(_http);

        var (pathologistId, endo1Id, endo2Id, _, _) =
            await PathologyTestSeed.SeedPathologistAndTwoEndoscopiesAsync(_factory, 10m, 5m);

        // Parcel 1 with endo1
        var parcel1Create = new PathologyParcelCreateRequestDto(
            PathologistId: new PathologistId(pathologistId),
            EndoscopyIds: new[] { new EndoscopyId(endo1Id) },
            DispatchedAtUtc: null,
            CourierName: null,
            TrackingNumber: null,
            Notes: null);

        var p1Resp = await _http.PostJsonAsync("/api/pathology/parcels", parcel1Create);
        p1Resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var p1Id = (await p1Resp.Content.ReadJsonAsync<ResultDto<PathologyParcelId>>())!.Value!.Value;

        // Parcel 2 with endo2 (different parcel id we will try to (incorrectly) set on the report)
        var parcel2Create = parcel1Create with { EndoscopyIds = new[] { new EndoscopyId(endo2Id) } };
        var p2Resp = await _http.PostJsonAsync("/api/pathology/parcels", parcel2Create);
        p2Resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var p2Id = (await p2Resp.Content.ReadJsonAsync<ResultDto<PathologyParcelId>>())!.Value!.Value;

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

        // Update a normal field (allowed) - omit DispatchParcelId
        var update1 = new PathologyReportUpsertRequestDto(
            Id: item.Id,
            PatientId: item.PatientId,
            EndoscopyId: item.EndoscopyId,
            PathologistId: item.PathologistId,
            DispatchParcelId: null,
            SentAtUtc: item.SentAtUtc,
            ReceivedAtUtc: item.ReceivedAtUtc,
            Notes: null,
            ClinicalInfo: null,
            MacroscopyText: null,
            DiagnosisText: "updated",
            Status: item.Status,
            IsUrgent: item.IsUrgent,
            DocumentFileId: null,
            DocumentKind: PathologyDocumentKind.Unknown,
            DocumentFileName: null,
            DocumentContentType: null,
            Document: null,
            RowVersion: item.RowVersion);

        var u1Resp = await _http.PutJsonAsync($"/api/pathology/reports/{item.Id.Value}", update1);
        u1Resp.StatusCode.Should().Be(HttpStatusCode.OK);

        // Grab latest rowversion
        var got = await _http.GetAsync($"/api/pathology/reports/{item.Id.Value}");
        got.StatusCode.Should().Be(HttpStatusCode.OK);
        var gotBody = (await got.Content.ReadJsonAsync<ResultDto<PathologyReportDto>>())!.Value!;

        // Now try to (incorrectly) change DispatchParcelId to parcel2
        var update2 = update1 with { DispatchParcelId = new PathologyParcelId(p2Id), RowVersion = gotBody.RowVersion };
        var u2Resp = await _http.PutJsonAsync($"/api/pathology/reports/{item.Id.Value}", update2);
        u2Resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var u2Body = await u2Resp.Content.ReadJsonAsync<ResultDto<bool>>();
        u2Body!.IsSuccess.Should().BeFalse();
        u2Body.Error!.Code.Should().Be("validation");
    }

    [Fact]
    public async Task DeleteReport_WhenInDispatchedParcel_ShouldReturn400()
    {
        await DevSeedHelper.SeedAsync(_http);

        var (pathologistId, endo1Id, _, _, _) =
            await PathologyTestSeed.SeedPathologistAndTwoEndoscopiesAsync(_factory, 10m, 5m);

        // Create dispatched parcel
        var parcelCreate = new PathologyParcelCreateRequestDto(
            PathologistId: new PathologistId(pathologistId),
            EndoscopyIds: new[] { new EndoscopyId(endo1Id) },
            DispatchedAtUtc: DateTime.UtcNow,
            CourierName: "Courier",
            TrackingNumber: "TRK",
            Notes: null);

        var pResp = await _http.PostJsonAsync("/api/pathology/parcels", parcelCreate);
        pResp.StatusCode.Should().Be(HttpStatusCode.OK);

        // Find report
        var search = new PathologyReportSearchRequestDto(
            EndoscopyId: new EndoscopyId(endo1Id),
            PathologistId: new PathologistId(pathologistId),
            Paging: new PagedRequestDto(1, 50));

        var sResp = await _http.PostJsonAsync("/api/pathology/reports/search", search);
        sResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var sBody = await sResp.Content.ReadJsonAsync<ResultDto<PagedResultDto<PathologyReportListItemDto>>>();
        var reportId = sBody!.Value!.Items.Single().Id.Value;

        var del = await _http.DeleteAsync($"/api/pathology/reports/{reportId}");
        del.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var delBody = await del.Content.ReadJsonAsync<ResultDto<bool>>();
        delBody!.IsSuccess.Should().BeFalse();
        delBody.Error!.Code.Should().Be("validation");
    }
}
