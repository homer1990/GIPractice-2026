using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;
using Xunit;

namespace GIPractice.Api.Tests;

public sealed class PathologyReportApiTests : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _http;

    public PathologyReportApiTests(TestApiFactory factory)
    {
        _http = factory.CreateClient();
    }

    [Fact]
    public async Task Search_ShouldReturn200_AndResultDtoShape()
    {
        await DevSeedHelper.SeedAsync(_http);

        var req = new PathologyReportSearchRequestDto(
            PatientId: null,
            EndoscopyId: null,
            Status: null,
            Paging: new PagedRequestDto(Page: 1, PageSize: 50));

        var resp = await _http.PostJsonAsync("/api/pathology/search", req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await resp.Content.ReadJsonAsync<ResultDto<PagedResultDto<PathologyReportListItemDto>>>();
        body.Should().NotBeNull();
        body!.IsSuccess.Should().BeTrue();
        body.Error.Should().BeNull();
        body.Value.Should().NotBeNull();
        body.Value!.TotalCount.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task Create_ThenGet_ShouldReturnReportDetails()
    {
        await DevSeedHelper.SeedAsync(_http);

        var create = new PathologyReportUpsertRequestDto(
            Id: null,
            PatientId: new PatientId(1),
            EndoscopyId: new EndoscopyId(1),
            BiopsyDispatchBundleId: null,
            LabName: "Lab A",
            ParcelId: "P-001",
            SentUtc: DateTime.UtcNow,
            ReceivedUtc: null,
            ClinicalInfo: "Clinical",
            Macroscopy: "Macro",
            DiagnosisText: "Dx",
            Status: PathologyReportStatus.Pending,
            IsUrgent: false,
            Notes: null,
            RowVersion: null);

        var post = await _http.PostJsonAsync("/api/pathology", create);
        post.StatusCode.Should().Be(HttpStatusCode.OK);

        var postBody = await post.Content.ReadJsonAsync<ResultDto<PathologyReportId>>();
        postBody!.IsSuccess.Should().BeTrue();
        postBody.Value.Should().NotBe(default(PathologyReportId));

        var id = postBody.Value;

        var get = await _http.GetAsync($"/api/pathology/{id.Value}");
        get.StatusCode.Should().Be(HttpStatusCode.OK);

        var getBody = await get.Content.ReadJsonAsync<ResultDto<PathologyReportDto>>();
        getBody!.IsSuccess.Should().BeTrue();
        getBody.Value.Should().NotBeNull();
        getBody.Value!.Id.Should().Be(id);
        getBody.Value.LabName.Should().Be("Lab A");
        getBody.Value.Status.Should().Be(PathologyReportStatus.Pending);
    }

    [Fact]
    public async Task Update_WithRouteId_ShouldSucceed_WhenBodyIdIsWrong()
    {
        await DevSeedHelper.SeedAsync(_http);

        // Create
        var create = new PathologyReportUpsertRequestDto(
            Id: null,
            PatientId: new PatientId(1),
            EndoscopyId: new EndoscopyId(1),
            BiopsyDispatchBundleId: null,
            LabName: "Lab B",
            ParcelId: "P-002",
            SentUtc: DateTime.UtcNow,
            ReceivedUtc: null,
            ClinicalInfo: "Clinical",
            Macroscopy: "Macro",
            DiagnosisText: "Dx",
            Status: PathologyReportStatus.Pending,
            IsUrgent: false,
            Notes: null,
            RowVersion: null);

        var post = await _http.PostJsonAsync("/api/pathology", create);
        var postBody = await post.Content.ReadJsonAsync<ResultDto<PathologyReportId>>();
        var id = postBody!.Value;

        // Get rowversion
        var get = await _http.GetAsync($"/api/pathology/{id.Value}");
        var getBody = await get.Content.ReadJsonAsync<ResultDto<PathologyReportDto>>();
        var cur = getBody!.Value!;

        // Update (body Id intentionally wrong)
        var update = new PathologyReportUpsertRequestDto(
            Id: new PathologyReportId(999),
            PatientId: cur.PatientId,
            EndoscopyId: cur.EndoscopyId,
            BiopsyDispatchBundleId: cur.BiopsyDispatchBundleId,
            LabName: "Lab B",
            ParcelId: cur.ParcelId,
            SentUtc: cur.SentUtc,
            ReceivedUtc: cur.ReceivedUtc,
            ClinicalInfo: cur.ClinicalInfo,
            Macroscopy: cur.Macroscopy,
            DiagnosisText: "UPDATED",
            Status: PathologyReportStatus.Completed,
            IsUrgent: true,
            Notes: cur.Notes,
            RowVersion: cur.RowVersion);

        var put = await _http.PutJsonAsync($"/api/pathology/{id.Value}", update);
        put.StatusCode.Should().Be(HttpStatusCode.OK);

        var get2 = await _http.GetAsync($"/api/pathology/{id.Value}");
        var get2Body = await get2.Content.ReadJsonAsync<ResultDto<PathologyReportDto>>();
        get2Body!.Value!.DiagnosisText.Should().Be("UPDATED");
        get2Body.Value.Status.Should().Be(PathologyReportStatus.Completed);
        get2Body.Value.IsUrgent.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_ThenGet_ShouldReturn404()
    {
        await DevSeedHelper.SeedAsync(_http);

        var create = new PathologyReportUpsertRequestDto(
            Id: null,
            PatientId: new PatientId(1),
            EndoscopyId: new EndoscopyId(1),
            BiopsyDispatchBundleId: null,
            LabName: "Lab C",
            ParcelId: "P-003",
            SentUtc: DateTime.UtcNow,
            ReceivedUtc: null,
            ClinicalInfo: "Clinical",
            Macroscopy: "Macro",
            DiagnosisText: "Dx",
            Status: PathologyReportStatus.Pending,
            IsUrgent: false,
            Notes: null,
            RowVersion: null);

        var post = await _http.PostJsonAsync("/api/pathology", create);
        var postBody = await post.Content.ReadJsonAsync<ResultDto<PathologyReportId>>();
        var id = postBody!.Value;

        var del = await _http.DeleteAsync($"/api/pathology/{id.Value}");
        del.StatusCode.Should().Be(HttpStatusCode.OK);

        var get = await _http.GetAsync($"/api/pathology/{id.Value}");
        get.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
