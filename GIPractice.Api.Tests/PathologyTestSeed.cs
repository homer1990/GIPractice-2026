using FluentAssertions;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathologists;
using GIPractice.Contracts.Pathology;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace GIPractice.Api.Tests;

public static class PathologyTestSeed
{
    public sealed record SeedResult(
        PathologistId PathologistId,
        string ParcelCode);

    public static async Task<SeedResult> EnsureAsync(HttpClient http)
    {
        // base seed (patient=1, endoscopy=1 etc)
        await DevSeedHelper.SeedAsync(http);

        // 1) Create Pathologist
        var createPathologist = await http.PostAsJsonAsync(
            "/api/pathologists",
            new PathologistUpsertRequestDto(
                Id: null,
                Name: "Seed Pathologist",
                Address: null,
                Email: "pathologist@seed.local",
                PhoneNumber: null,
                PricingPlanJson: "{}",
                RowVersion: null));

        createPathologist.StatusCode.Should().Be(HttpStatusCode.OK);

        var pathBody = await createPathologist.Content.ReadFromJsonAsync<ResultDto<PathologistId>>();
        pathBody!.IsSuccess.Should().BeTrue();
        var pathologistId = pathBody.Value;

        // 2) Create Parcel (server generates ParcelCode or you pass it, depending on your contract)
        // If your Create returns PathologyParcelDto instead, tweak the read target accordingly.
        var createParcel = await http.PostAsJsonAsync(
            "/api/pathology/parcels",
            new PathologyParcelCreateRequestDto(
                PathologistId: pathologistId,
                Notes: "Seed parcel"));

        createParcel.StatusCode.Should().Be(HttpStatusCode.OK);

        var parcelBody = await createParcel.Content.ReadFromJsonAsync<ResultDto<PathologyParcelDto>>();
        parcelBody!.IsSuccess.Should().BeTrue();
        var parcel = parcelBody.Value!;
        parcel.PathologistId.Should().Be(pathologistId);

        // 3) Create Report (minimal)
        var createReport = await http.PostAsJsonAsync(
            "/api/pathology/reports",
            new PathologyReportUpsertRequestDto(
                Id: null,
                PatientId: new PatientId(1),
                EndoscopyId: new EndoscopyId(1),
                PathologistId: pathologistId,
                DispatchParcelId: null,
                SentAtUtc: null,
                ReceivedAtUtc: null,
                Notes: "Seed report",
                ClinicalInfo: null,
                MacroscopyText: null,
                DiagnosisText: null,
                Status: PathologyReportStatus.Dispatched,
                IsUrgent: false,
                Document: null,
                DocumentFileId: null,
                DocumentKind: PathologyDocumentKind.Unknown,
                DocumentFileName: null,
                DocumentContentType: null,
                RowVersion: null));

        createReport.StatusCode.Should().Be(HttpStatusCode.OK);

        var reportBody = await createReport.Content.ReadFromJsonAsync<ResultDto<PathologyReportId>>();
        reportBody!.IsSuccess.Should().BeTrue();
        var reportId = reportBody.Value;

        // 4) Assign report to parcel
        var assign = await http.PostAsJsonAsync(
            $"/api/pathology/parcels/{pathologistId.Value}/{parcel.ParcelCode}/reports/assign",
            new PathologyParcelAssignReportsRequestDto(new PathologyReportId[] { reportId }));

        assign.StatusCode.Should().Be(HttpStatusCode.OK);

        var assignBody = await assign.Content.ReadFromJsonAsync<ResultDto<bool>>();
        assignBody!.IsSuccess.Should().BeTrue();

        return new SeedResult(pathologistId, parcel.ParcelCode);
    }
}
