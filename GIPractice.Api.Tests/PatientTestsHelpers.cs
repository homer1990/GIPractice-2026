using System.Net.Http.Json;
using FluentAssertions;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Patients;

namespace GIPractice.Api.Tests;

public static class PatientsTestHelpers
{
    public static async Task<PatientId> CreatePatientAsync(HttpClient http, string lastName, string firstName)
    {
        var req = new PatientUpsertRequestDto(
            Id: null,
            LastName: lastName,
            FirstName: firstName,
            FathersName: null,
            PersonalNumber: MakeUniquePersonalNumber(),
            BirthDate: null,
            Gender: null,
            Email: null,
            PhoneNumber: null,
            Address: null,
            HasHadCA: false,
            HasHadIBD: false,
            HasPendingBiopsies: false,
            HasScheduledEndo: false,
            PhotoBytes: null,
            PhotoContentType: null,
            RowVersion: null);

        var resp = await http.PostAsJsonAsync("/api/patients", req);
        resp.EnsureSuccessStatusCode();

        // NOTE: server serializes strong IDs as numbers => read as int
        var created = await resp.Content.ReadFromJsonAsync<ResultDto<int>>();
        created.Should().NotBeNull();
        created!.IsSuccess.Should().BeTrue();
        created.Value.Should().BeGreaterThan(0);

        return new PatientId(created.Value);
    }

    private static string MakeUniquePersonalNumber()
    {
        // 12 digits; good enough for tests
        var n = Random.Shared.NextInt64(1_000_000_000_000L);
        return n.ToString("000000000000");
    }
}
