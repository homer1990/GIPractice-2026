using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Patients;

namespace GIPractice.Api.Tests;

public sealed class PatientsCrudApiTests(TestApiFactory factory) : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _http = factory.CreateClient();

    [Fact]
    public async Task Create_ThenGet_ShouldReturnPatientDetails()
    {
        var (id, pn) = await PatientsTestHelpers.CreatePatientAsync(
            _http,
            lastName: "Papadopoulos",
            firstName: "Giorgos");

        var getResp = await _http.GetAsync($"/api/patients/{id.Value}");
        getResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await getResp.Content.ReadFromJsonAsync<ResultDto<PatientDetailsDto>>();
        dto.Should().NotBeNull();
        dto!.IsSuccess.Should().BeTrue();

        dto.Value.Id.Should().Be(id);
        dto.Value.LastName.Should().Be("Papadopoulos");
        dto.Value.FirstName.Should().Be("Giorgos");
        dto.Value.PersonalNumber.Should().Be(pn);
    }

    [Fact]
    public async Task Search_ByLastName_ShouldReturnCreatedPatient()
    {
        var (id, _) = await PatientsTestHelpers.CreatePatientAsync(
            _http,
            lastName: "ZetaSmith",
            firstName: "Alex");

        var req = new PatientSearchRequestDto(LastName: "ZetaSmith");

        var resp = await _http.PostAsJsonAsync("/api/patients/search", req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await resp.Content.ReadFromJsonAsync<ResultDto<PagedResultDto<PatientListItemDto>>>();
        dto.Should().NotBeNull();
        dto!.IsSuccess.Should().BeTrue();

        dto.Value.Total.Should().BeGreaterThan(0);
        dto.Value.Items.Select(x => x.Id).Should().Contain(id);
    }

    [Fact]
    public async Task Update_RouteIdWins_WhenBodyIdDiffers()
    {
        var (id1, pn1) = await PatientsTestHelpers.CreatePatientAsync(_http, "Alpha", "One");
        var (id2, pn2) = await PatientsTestHelpers.CreatePatientAsync(_http, "Beta", "Two");

        // PUT /api/patients/{id1} but send Id=id2 in body.
        // Controller should override and update id1.
        var updateReq = new PatientUpsertRequestDto(
            Id: id2,
            LastName: "Alpha-Updated",
            FirstName: "One",
            FathersName: null,
            BirthDate: null,
            PersonalNumber: pn1,
            Gender: null,
            PhoneNumber: null,
            Email: null,
            Address: null,
            HasHadCA: false,
            HasHadIBD: false,
            HasPendingBiopsies: false,
            HasScheduledEndo: false,
            PhotoBytes: null,
            PhotoContentType: null,
            RowVersion: null);

        var put = await _http.PutAsJsonAsync($"/api/patients/{id1.Value}", updateReq);
        put.StatusCode.Should().Be(HttpStatusCode.OK);

        var putDto = await put.Content.ReadFromJsonAsync<ResultDto<bool>>();
        putDto.Should().NotBeNull();
        putDto!.IsSuccess.Should().BeTrue();
        putDto.Value.Should().BeTrue();

        // id1 should be updated
        var g1 = await _http.GetFromJsonAsync<ResultDto<PatientDetailsDto>>($"/api/patients/{id1.Value}");
        g1!.IsSuccess.Should().BeTrue();
        g1.Value.LastName.Should().Be("Alpha-Updated");
        g1.Value.PersonalNumber.Should().Be(pn1);

        // id2 should be unchanged
        var g2 = await _http.GetFromJsonAsync<ResultDto<PatientDetailsDto>>($"/api/patients/{id2.Value}");
        g2!.IsSuccess.Should().BeTrue();
        g2.Value.LastName.Should().Be("Beta");
        g2.Value.PersonalNumber.Should().Be(pn2);
    }

    [Fact]
    public async Task Delete_ThenGet_ShouldReturn404()
    {
        var (id, _) = await PatientsTestHelpers.CreatePatientAsync(_http, "ToDelete", "Guy");

        var del = await _http.DeleteAsync($"/api/patients/{id.Value}");
        del.StatusCode.Should().Be(HttpStatusCode.OK);

        var delDto = await del.Content.ReadFromJsonAsync<ResultDto<bool>>();
        delDto.Should().NotBeNull();
        delDto!.IsSuccess.Should().BeTrue();
        delDto.Value.Should().BeTrue();

        var get = await _http.GetAsync($"/api/patients/{id.Value}");
        get.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var getDto = await get.Content.ReadFromJsonAsync<ResultDto<PatientDetailsDto>>();
        getDto.Should().NotBeNull();
        getDto!.IsSuccess.Should().BeFalse();
        getDto.Error!.Code.Should().Be("not_found");
    }
}
