using System.Globalization;
using System.Net.Http.Json;
using System.Threading;
using FluentAssertions;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Patients;

namespace GIPractice.Api.Tests;

internal static class PatientsTestHelpers
{
    // 12-digit Greek Personal Number placeholder in this project.
    // Use a deterministic increment so parallel tests don't collide.
    private static long _pnCounter = DateTime.UtcNow.Ticks % 1_000_000_000_000L;

    private static string NextPersonalNumber()
    {
        var n = Interlocked.Increment(ref _pnCounter) % 1_000_000_000_000L;
        return n.ToString("D12", CultureInfo.InvariantCulture);
    }

    public static async Task<(PatientId Id, string PersonalNumber)> CreatePatientAsync(
        HttpClient http,
        string lastName,
        string firstName)
    {
        var pn = NextPersonalNumber();

        var req = new PatientUpsertRequestDto(
            Id: null,
            LastName: lastName,
            FirstName: firstName,
            FathersName: null,
            BirthDate: null,
            PersonalNumber: pn,
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

        var resp = await http.PostAsJsonAsync("/api/patients", req);
        resp.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var dto = await resp.Content.ReadFromJsonAsync<ResultDto<PatientId>>();
        dto.Should().NotBeNull();
        dto!.IsSuccess.Should().BeTrue();
        dto.Value.Value.Should().BeGreaterThan(0);

        return (dto.Value, pn);
    }
}
