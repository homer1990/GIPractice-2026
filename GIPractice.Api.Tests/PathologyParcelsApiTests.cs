using FluentAssertions;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Xunit;

namespace GIPractice.Api.Tests;

public sealed class PathologyParcelsApiTests(TestApiFactory factory) : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _http = factory.CreateClient();
    private readonly TestApiFactory _factory = factory;

    [Fact]
    public async Task Create_ShouldCalculateMonetarySum_FromEndoscopyBiopsiesCost()
    {
        await DevSeedHelper.SeedAsync(_http);

        var (pathologistId, endo1Id, endo2Id, cost1, cost2) =
            await PathologyTestSeed.SeedPathologistAndTwoEndoscopiesAsync(_factory, 50m, 70m);

        var create = new PathologyParcelCreateRequestDto(
            PathologistId: new PathologistId(pathologistId),
            EndoscopyIds: [new EndoscopyId(endo1Id), new EndoscopyId(endo2Id)],
            DispatchedAtUtc: null,
            CourierName: null,
            TrackingNumber: null,
            Notes: "test");

        var resp = await _http.PostJsonAsync("/api/pathology/parcels", create);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await resp.Content.ReadJsonAsync<ResultDto<PathologyParcelId>>();
        body!.IsSuccess.Should().BeTrue();
        body.Value.Should().NotBeNull();

        var parcelId = body.Value!.Value;

        var get = await _http.GetAsync($"/api/pathology/parcels/{parcelId}");
        get.StatusCode.Should().Be(HttpStatusCode.OK);

        var getBody = await get.Content.ReadJsonAsync<ResultDto<PathologyParcelDto>>();
        getBody!.IsSuccess.Should().BeTrue();
        getBody.Value!.MonetarySum.Should().Be(cost1 + cost2);
    }

    [Fact]
    public async Task Assign_And_Unassign_ShouldUpdateMonetarySum()
    {
        await DevSeedHelper.SeedAsync(_http);

        var (pathologistId, endo1Id, endo2Id, cost1, cost2) =
            await PathologyTestSeed.SeedPathologistAndTwoEndoscopiesAsync(_factory, 40m, 25m);

        var create = new PathologyParcelCreateRequestDto(
            PathologistId: new PathologistId(pathologistId),
            EndoscopyIds: [new EndoscopyId(endo1Id)],
            DispatchedAtUtc: null,
            CourierName: null,
            TrackingNumber: null,
            Notes: null);

        var createResp = await _http.PostJsonAsync("/api/pathology/parcels", create);
        createResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var createBody = await createResp.Content.ReadJsonAsync<ResultDto<PathologyParcelId>>();
        createBody!.IsSuccess.Should().BeTrue();
        var parcelId = createBody.Value!.Value;

        var parcel = (await (await _http.GetAsync($"/api/pathology/parcels/{parcelId}"))
            .Content.ReadJsonAsync<ResultDto<PathologyParcelDto>>())!.Value!;
        parcel.MonetarySum.Should().Be(cost1);

        var keyUrl = $"/api/pathology/parcels/{pathologistId}/{parcel.ParcelCode}";

        // Assign route: POST .../items
        var assignReq = new PathologyParcelAssignEndoscopiesRequestDto([new EndoscopyId(endo2Id)]);
        var assignResp = await _http.PostJsonAsync($"{keyUrl}/items", assignReq);
        assignResp.StatusCode.Should().Be(HttpStatusCode.OK);
        (await assignResp.Content.ReadJsonAsync<ResultDto<bool>>())!.IsSuccess.Should().BeTrue();

        var afterAssign = (await (await _http.GetAsync(keyUrl))
            .Content.ReadJsonAsync<ResultDto<PathologyParcelDto>>())!.Value!;
        afterAssign.MonetarySum.Should().Be(cost1 + cost2);

        // Unassign route: POST .../items/unassign
        var unassignReq = new PathologyParcelAssignEndoscopiesRequestDto([new EndoscopyId(endo1Id)]);
        var unassignResp = await _http.PostJsonAsync($"{keyUrl}/items/unassign", unassignReq);
        unassignResp.StatusCode.Should().Be(HttpStatusCode.OK);
        (await unassignResp.Content.ReadJsonAsync<ResultDto<bool>>())!.IsSuccess.Should().BeTrue();

        var afterUnassign = (await (await _http.GetAsync(keyUrl))
            .Content.ReadJsonAsync<ResultDto<PathologyParcelDto>>())!.Value!;
        afterUnassign.MonetarySum.Should().Be(cost2);
    }

    [Fact]
    public async Task UpdateByKey_ShouldRecalculateMonetarySum_WhenEndoscopyCostChanges()
    {
        await DevSeedHelper.SeedAsync(_http);

        var (pathologistId, endo1Id, _, cost1, _) =
            await PathologyTestSeed.SeedPathologistAndTwoEndoscopiesAsync(_factory, 10m, 0m);

        var create = new PathologyParcelCreateRequestDto(
            PathologistId: new PathologistId(pathologistId),
            EndoscopyIds: [new EndoscopyId(endo1Id)],
            DispatchedAtUtc: null,
            CourierName: null,
            TrackingNumber: null,
            Notes: null);

        var createResp = await _http.PostJsonAsync("/api/pathology/parcels", create);
        createResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var createBody = await createResp.Content.ReadJsonAsync<ResultDto<PathologyParcelId>>();
        createBody!.IsSuccess.Should().BeTrue();
        var parcelId = createBody.Value!.Value;

        var get1 = (await (await _http.GetAsync($"/api/pathology/parcels/{parcelId}"))
            .Content.ReadJsonAsync<ResultDto<PathologyParcelDto>>())!.Value!;
        get1.MonetarySum.Should().Be(cost1);

        var newCost = 99m;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GIPractice.Infrastructure.AppDbContext>();
            var endo = await db.Endoscopies.FindAsync(endo1Id);
            endo!.BiopsiesCost = newCost;
            await db.SaveChangesAsync();
        }

        var keyUrl = $"/api/pathology/parcels/{pathologistId}/{get1.ParcelCode}";
        var updateReq = new PathologyParcelUpdateRequestDto(
            Id: get1.Id,
            DispatchedAtUtc: get1.DispatchedAtUtc,
            CourierName: get1.CourierName,
            TrackingNumber: get1.TrackingNumber,
            Notes: get1.Notes,
            RowVersion: get1.RowVersion);

        var put = await _http.PutJsonAsync(keyUrl, updateReq);
        put.StatusCode.Should().Be(HttpStatusCode.OK);
        (await put.Content.ReadJsonAsync<ResultDto<bool>>())!.IsSuccess.Should().BeTrue();

        var get2 = (await (await _http.GetAsync(keyUrl))
            .Content.ReadJsonAsync<ResultDto<PathologyParcelDto>>())!.Value!;
        get2.MonetarySum.Should().Be(newCost);
    }
}
