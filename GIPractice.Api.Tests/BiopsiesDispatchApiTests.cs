using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using GIPractice.Contracts.Biopsies;
using GIPractice.Contracts.Common;
using Xunit;

namespace GIPractice.Api.Tests;

public sealed class BiopsiesDispatchApiTests : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _http;

    public BiopsiesDispatchApiTests(TestApiFactory factory)
    {
        _http = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task Dispatch_CreateThenSearch_ShouldReturnBundle()
    {
        await DevSeedHelper.SeedAsync(_http);

        var create = await _http.PostAsJsonAsync(
            "/api/biopsies/dispatch",
            new BiopsyDispatchCreateRequestDto(ProtocolNumber: "PROTO-001", Notes: "test"),
            TestJson.Options);

        create.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await create.Content.ReadFromJsonAsync<ResultDto<GIPractice.Contracts.Ids.BiopsyDispatchBundleId>>(TestJson.Options);
        created!.IsSuccess.Should().BeTrue();

        var search = await _http.PostAsJsonAsync(
            "/api/biopsies/dispatch/search",
            new BiopsyDispatchSearchRequestDto(ProtocolNumber: "PROTO-001", IsClosed: null, CreatedFrom: null, CreatedTo: null, Paging: new PagedRequestDto(1, 50)),
            TestJson.Options);

        search.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await search.Content.ReadFromJsonAsync<ResultDto<PagedResultDto<BiopsyDispatchBundleDto>>>(TestJson.Options);
        body!.IsSuccess.Should().BeTrue();
        body.Value!.Items.Should().ContainSingle(x => x.ProtocolNumber == "PROTO-001");
    }

    [Fact]
    public async Task Dispatch_Close_ShouldMarkBundleClosed()
    {
        await DevSeedHelper.SeedAsync(_http);

        // create
        var create = await _http.PostAsJsonAsync(
            "/api/biopsies/dispatch",
            new BiopsyDispatchCreateRequestDto(ProtocolNumber: "PROTO-002"),
            TestJson.Options);

        var created = await create.Content.ReadFromJsonAsync<ResultDto<GIPractice.Contracts.Ids.BiopsyDispatchBundleId>>(TestJson.Options);
        var id = created!.Value!;

        // close via route id (body id can be null)
        var close = await _http.PutAsJsonAsync(
            $"/api/biopsies/dispatch/{id.Value}/close",
            new BiopsyDispatchCloseRequestDto(Id: null, RowVersion: null),
            TestJson.Options);

        close.StatusCode.Should().Be(HttpStatusCode.OK);

        // search closed
        var searchClosed = await _http.PostAsJsonAsync(
            "/api/biopsies/dispatch/search",
            new BiopsyDispatchSearchRequestDto(ProtocolNumber: "PROTO-002", IsClosed: true, Paging: new PagedRequestDto(1, 50)),
            TestJson.Options);

        var body = await searchClosed.Content.ReadFromJsonAsync<ResultDto<PagedResultDto<BiopsyDispatchBundleDto>>>(TestJson.Options);
        body!.Value!.Items.Should().ContainSingle(x => x.ProtocolNumber == "PROTO-002" && x.IsClosed);
    }
}
