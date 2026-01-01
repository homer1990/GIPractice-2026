using System.Net;
using FluentAssertions;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathologists;
using GIPractice.Contracts.Pathology;
using Xunit;

namespace GIPractice.Api.Tests;

public sealed class PathologistsApiTests(TestApiFactory factory) : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _http = factory.CreateAuthenticatedClient();
    private readonly TestApiFactory _factory = factory;

    [Fact]
    public async Task Create_Get_Update_RowVersionConflict_Delete_Flow()
    {
        var create = new PathologistUpsertRequestDto(
            Id: null,
            Name: $"Dr Test {Guid.NewGuid():N}",
            Address: "Addr",
            Email: "test@example.com",
            PhoneNumber: "2100000000",
            PricingPlanJson: "{}",
            RowVersion: null);

        var post = await _http.PostJsonAsync("/api/pathologists", create);
        post.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await post.Content.ReadJsonAsync<ResultDto<PathologistId>>();
        created!.IsSuccess.Should().BeTrue();
        var id = created.Value!.Value;

        var get1Resp = await _http.GetAsync($"/api/pathologists/{id}");
        get1Resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var get1 = await get1Resp.Content.ReadJsonAsync<ResultDto<PathologistDto>>();
        get1!.IsSuccess.Should().BeTrue();
        get1.Value!.Id.Value.Should().Be(id);
        get1.Value!.RowVersion.Should().NotBeNull();

        var rv1 = get1.Value!.RowVersion;

        var update = create with
        {
            Id = new PathologistId(id + 9999), // route wins
            Name = create.Name + " Updated",
            Email = "test2@example.com",
            RowVersion = rv1
        };

        var put = await _http.PutJsonAsync($"/api/pathologists/{id}", update);
        put.StatusCode.Should().Be(HttpStatusCode.OK);
        (await put.Content.ReadJsonAsync<ResultDto<bool>>())!.IsSuccess.Should().BeTrue();

        var get2 = (await (await _http.GetAsync($"/api/pathologists/{id}")).Content.ReadJsonAsync<ResultDto<PathologistDto>>())!.Value!;
        get2.Name.Should().EndWith("Updated");
        get2.RowVersion.Should().NotBeNull();

        // Re-use old RowVersion should conflict
        var conflictUpdate = update with { RowVersion = rv1 };
        var put2 = await _http.PutJsonAsync($"/api/pathologists/{id}", conflictUpdate);
        put2.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var put2Body = await put2.Content.ReadJsonAsync<ResultDto<bool>>();
        put2Body!.IsSuccess.Should().BeFalse();
        put2Body.Error!.Code.Should().Be("conflict");

        var del = await _http.DeleteAsync($"/api/pathologists/{id}");
        del.StatusCode.Should().Be(HttpStatusCode.OK);
        (await del.Content.ReadJsonAsync<ResultDto<bool>>())!.IsSuccess.Should().BeTrue();

        var getDeleted = await _http.GetAsync($"/api/pathologists/{id}");
        getDeleted.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithInvalidPricingPlanJson_ShouldReturn400()
    {
        var create = new PathologistUpsertRequestDto(
            Id: null,
            Name: "Dr InvalidJson",
            Address: null,
            Email: null,
            PhoneNumber: null,
            PricingPlanJson: "{",
            RowVersion: null);

        var post = await _http.PostJsonAsync("/api/pathologists", create);
        post.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await post.Content.ReadJsonAsync<ResultDto<PathologistId>>();
        body!.IsSuccess.Should().BeFalse();
        body.Error!.Code.Should().Be("validation");
    }

    [Fact]
    public async Task Search_ByName_ShouldReturnMatches()
    {
        var name1 = $"Alpha {Guid.NewGuid():N}";
        var name2 = $"Beta {Guid.NewGuid():N}";

        await _http.PostJsonAsync("/api/pathologists", new PathologistUpsertRequestDto(
            Id: null,
            Name: name1,
            Address: null,
            Email: null,
            PhoneNumber: null,
            PricingPlanJson: "{}",
            RowVersion: null));

        await _http.PostJsonAsync("/api/pathologists", new PathologistUpsertRequestDto(
            Id: null,
            Name: name2,
            Address: null,
            Email: null,
            PhoneNumber: null,
            PricingPlanJson: "{}",
            RowVersion: null));

        var search = new PathologistSearchRequestDto(Name: "Alpha", Paging: new PagedRequestDto(1, 50));
        var resp = await _http.PostJsonAsync("/api/pathologists/search", search);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await resp.Content.ReadJsonAsync<ResultDto<PagedResultDto<PathologistListItemDto>>>();
        body!.IsSuccess.Should().BeTrue();
        body.Value!.Items.Should().Contain(x => x.Name == name1);
        body.Value!.Items.Should().NotContain(x => x.Name == name2);
    }

    [Fact]
    public async Task Delete_WhenReferencedByParcel_ShouldReturn400()
    {
        await DevSeedHelper.SeedAsync(_http);

        var (pathologistId, endo1Id, _, _, _) =
            await PathologyTestSeed.SeedPathologistAndTwoEndoscopiesAsync(_factory, 10m, 5m);

        var createParcel = new PathologyParcelCreateRequestDto(
            PathologistId: new PathologistId(pathologistId),
            EndoscopyIds: new[] { new EndoscopyId(endo1Id) },
            DispatchedAtUtc: null,
            CourierName: null,
            TrackingNumber: null,
            Notes: "x");

        var parcelResp = await _http.PostJsonAsync("/api/pathology/parcels", createParcel);
        parcelResp.StatusCode.Should().Be(HttpStatusCode.OK);
        (await parcelResp.Content.ReadJsonAsync<ResultDto<PathologyParcelId>>())!.IsSuccess.Should().BeTrue();

        var del = await _http.DeleteAsync($"/api/pathologists/{pathologistId}");
        del.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var delBody = await del.Content.ReadJsonAsync<ResultDto<bool>>();
        delBody!.IsSuccess.Should().BeFalse();
        delBody.Error!.Code.Should().Be("validation");
    }
}
