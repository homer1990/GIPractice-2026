using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using GIPractice.Contracts.Patients;
using Xunit;

namespace GIPractice.Api.Tests;

public sealed class PatientsRouteIdOverrideApiTests(TestApiFactory factory) : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _http = factory.CreateClient();

    [Fact]
    public async Task UpdatePatient_WhenBodyIdDiffers_RouteIdStillWins()
    {
        var id = await CreatePatientAsync(lastName: "RouteWins", firstName: "Test");

        var update = new PatientUpsertRequestDto(
            Id: new GIPractice.Contracts.Ids.PatientId(id + 9999), // intentionally wrong
            LastName: "RouteWinsUpdated",
            FirstName: "Test",
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

        var put = await _http.PutAsJsonAsync($"/api/patients/{id}", update);
        put.StatusCode.Should().Be(HttpStatusCode.OK);

        var got = await _http.GetAsync($"/api/patients/{id}");
        got.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await got.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        doc.RootElement.GetProperty("isSuccess").GetBoolean().Should().BeTrue();
        var value = doc.RootElement.GetProperty("value");
        value.GetProperty("lastName").GetString().Should().Be("RouteWinsUpdated");
    }

    private async Task<int> CreatePatientAsync(string lastName, string firstName)
    {
        // personal number optional for now; keep request minimal.
        var req = new PatientUpsertRequestDto(
            Id: null,
            LastName: lastName,
            FirstName: firstName,
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
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        // server serializes strong IDs as numbers; be resilient if it ever becomes { value: n }
        var json = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.GetProperty("isSuccess").GetBoolean().Should().BeTrue();

        var v = doc.RootElement.GetProperty("value");
        return v.ValueKind switch
        {
            JsonValueKind.Number => v.GetInt32(),
            JsonValueKind.Object => v.GetProperty("value").GetInt32(),
            _ => throw new InvalidOperationException("Unexpected create patient response shape")
        };
    }
}
