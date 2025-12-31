using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Pathology;
using Xunit;
using GIPractice.Api.Tests;

namespace GIPractice.Api.Tests;

public sealed class PathologyParcelsApiTests : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _http;

    public PathologyParcelsApiTests(TestApiFactory factory) => _http = factory.CreateClient();

    [Fact]
    public async Task Parcel_Create_ThenGetByKey_ShouldWork()
    {
        var seed = await PathologyTestSeed.EnsureAsync(_http);

        var get = await _http.GetAsync($"/api/pathology/parcels/{seed.PathologistId.Value}/{seed.ParcelCode}");
        get.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await get.Content.ReadFromJsonAsync<ResultDto<PathologyParcelDto>>();
        body!.IsSuccess.Should().BeTrue();
        body.Value!.ParcelCode.Should().Be(seed.ParcelCode);
    }

    [Fact]
    public async Task Parcel_Search_ShouldReturn200_AndList()
    {
        var seed = await PathologyTestSeed.EnsureAsync(_http);

        var resp = await _http.PostAsJsonAsync(
            "/api/pathology/parcels/search",
            new PathologyParcelSearchRequestDto(PathologistId: seed.PathologistId, Paging: new PagedRequestDto(1, 50)));

        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await resp.Content.ReadFromJsonAsync<ResultDto<PagedResultDto<PathologyParcelListItemDto>>>();
        body!.IsSuccess.Should().BeTrue();
        body.Value!.Items.Should().Contain(x => x.ParcelCode == seed.ParcelCode);
    }

    [Fact]
    public async Task Parcel_UnassignReports_ShouldClearReportParcel()
    {
        var seed = await PathologyTestSeed.EnsureAsync(_http);

        // Find the report we assigned (search by parcel)
        var searchReports = await _http.PostAsJsonAsync(
            "/api/pathology/reports/search",
            new PathologyReportSearchRequestDto(DispatchParcelCode: seed.ParcelCode, Paging: new PagedRequestDto(1, 50)));

        var searchBody = await searchReports.Content.ReadFromJsonAsync<ResultDto<PagedResultDto<PathologyReportListItemDto>>>();
        searchBody!.IsSuccess.Should().BeTrue();
        searchBody.Value!.Items.Count.Should().BeGreaterThan(0);

        var reportId = searchBody.Value.Items[0].Id;

        // Unassign
        var unassign = await _http.PostAsJsonAsync(
            $"/api/pathology/parcels/{seed.PathologistId.Value}/{seed.ParcelCode}/reports/unassign",
            new PathologyParcelAssignReportsRequestDto(new[] { reportId }));

        unassign.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify report no longer returned by parcel search
        var searchReports2 = await _http.PostAsJsonAsync(
            "/api/pathology/reports/search",
            new PathologyReportSearchRequestDto(DispatchParcelCode: seed.ParcelCode, Paging: new PagedRequestDto(1, 50)));

        var body2 = await searchReports2.Content.ReadFromJsonAsync<ResultDto<PagedResultDto<PathologyReportListItemDto>>>();
        body2!.IsSuccess.Should().BeTrue();
        body2.Value!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Report_SearchByParcel_ShouldReturnAssigned()
    {
        var seed = await PathologyTestSeed.EnsureAsync(_http);

        var resp = await _http.PostAsJsonAsync(
            "/api/pathology/reports/search",
            new PathologyReportSearchRequestDto(DispatchParcelCode: seed.ParcelCode, Paging: new PagedRequestDto(1, 50)));

        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await resp.Content.ReadFromJsonAsync<ResultDto<PagedResultDto<PathologyReportListItemDto>>>();
        body!.IsSuccess.Should().BeTrue();
        body.Value!.Items.Count.Should().BeGreaterThan(0);
    }
}
