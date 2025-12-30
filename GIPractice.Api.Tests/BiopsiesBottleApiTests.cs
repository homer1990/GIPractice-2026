using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using GIPractice.Contracts.Biopsies;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using Xunit;

namespace GIPractice.Api.Tests;

public sealed class BiopsiesBottleApiTests : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _http;

    public BiopsiesBottleApiTests(TestApiFactory factory)
    {
        _http = factory.CreateClient();
    }

    [Fact]
    public async Task SearchBottles_ShouldReturn200_AndResultDtoShape()
    {
        await DevSeedHelper.SeedAsync(_http);

        var req = new BiopsyBottleSearchRequestDto(
            PatientId: null,
            EndoscopyId: null,
            IsUrgent: null,
            Paging: new PagedRequestDto(Page: 1, PageSize: 50));

        var resp = await _http.PostJsonAsync("/api/biopsies/bottles/search", req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await resp.Content.ReadJsonAsync<ResultDto<PagedResultDto<BiopsyBottleDto>>>();
        body.Should().NotBeNull();
        body!.IsSuccess.Should().BeTrue();
        body.Error.Should().BeNull();
        body.Value.Should().NotBeNull();

        // Ensure strong ids deserialize
        body.Value.Items[0].Id.Should().NotBe(default(BiopsyBottleId));
        body.Value.Items[0].PatientId.Should().NotBe(default(PatientId));
        body.Value.Items[0].EndoscopyId.Should().NotBe(default(EndoscopyId));
    }

    [Fact]
    public async Task UpdateBottle_WithRouteId_ShouldSucceed_WhenBodyIdIsNull()
    {
        await DevSeedHelper.SeedAsync(_http);

        // get one bottle id
        var search = await _http.PostAsJsonAsync(
            "/api/biopsies/bottles/search",
            new BiopsyBottleSearchRequestDto(Paging: new PagedRequestDto(1, 50)),
            TestJson.Options);

        var searchBody = await search.Content.ReadJsonAsync<ResultDto<PagedResultDto<BiopsyBottleDto>>>();

        var b = searchBody!.Value!.Items[0];

        var get = await _http.GetAsync($"/api/biopsies/bottles/{b.Id.Value}");
        get.StatusCode.Should().Be(HttpStatusCode.OK);
        var getBody = await get.Content.ReadJsonAsync<ResultDto<BiopsyBottleDto>>();
        var cur = getBody!.Value!;

        var updated = new BiopsyBottleUpsertRequestDto(
            Id: null, // should be overridden from route
            PatientId: cur.PatientId,
            EndoscopyId: cur.EndoscopyId,
            LabelCode: "UPDATED",
            SiteDescription: cur.SiteDescription,
            IsUrgent: cur.IsUrgent,
            Notes: cur.Notes,
            RowVersion: cur.RowVersion);

        var put = await _http.PutJsonAsync($"/api/biopsies/bottles/{b.Id.Value}", updated);

        put.StatusCode.Should().Be(HttpStatusCode.OK);

        var putBody = await put.Content.ReadJsonAsync<ResultDto<bool>>();


        putBody!.IsSuccess.Should().BeTrue();

        var get2 = await _http.GetAsync($"/api/biopsies/bottles/{b.Id.Value}");
        var get2Body = await get2.Content.ReadJsonAsync<ResultDto<BiopsyBottleDto>>();
        get2Body!.Value!.LabelCode.Should().Be("UPDATED");
    }
}
