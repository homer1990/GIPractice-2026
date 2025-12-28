using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Patients;
using Xunit;

namespace GIPractice.Api.Tests;

public sealed class PatientsApiTests(TestApiFactory factory) : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _http = factory.CreateClient();

    [Fact]
    public async Task Search_ShouldReturn200_AndResultDtoShape()
    {
        var req = new PatientSearchRequestDto(
            FirstName: null,
            LastName: null,
            FathersName: null,
            PersonalNumber: null,
            Phone: null,
            Email: null,
            HasHadCA: null,
            HasHadIBD: null,
            HasPendingBiopsies: null,
            HasScheduledEndo: null,
            Paging: null);

        var resp = await _http.PostAsJsonAsync("/api/patients/search", req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await resp.Content.ReadFromJsonAsync<ResultDto<PagedResultDto<PatientListItemDto>>>();
        dto.Should().NotBeNull();
        dto!.IsSuccess.Should().BeTrue();
        dto.Value.Should().NotBeNull();
    }
}
