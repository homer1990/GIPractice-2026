using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GIPractice.Contracts.Patients;
using Xunit;

namespace GIPractice.Api.Tests;

public sealed class PatientsValidationApiTests(TestApiFactory factory) : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _http = factory.CreateAuthenticatedClient();

    [Fact]
    public async Task CreatePatient_EmptyLastName_ShouldReturn400()
    {
        var req = new PatientUpsertRequestDto(
            Id: null,
            LastName: "", // invalid
            FirstName: "John",
            FathersName: null,
            BirthDate: null,
            PersonalNumber: null,
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

        var resp = await _http.PostAsJsonAsync("/api/patients", req);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // optional: make sure we got model validation and not a ResultDto error
        var body = await resp.Content.ReadAsStringAsync();
        body.Should().Contain("errors");
    }
}
