using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Scheduling;

namespace GIPractice.Api.Tests;

public sealed class SchedulingValidationTests(TestApiFactory factory) : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _http = factory.CreateAuthenticatedClient();

    [Fact]
    public async Task CreateAppointment_DefaultStartUtc_ShouldReturn400()
    {
        var req = new AppointmentUpsertRequestDto(
            Id: null,
            PatientId: new PatientId(1),
            StartUtc: default,                  // <-- should fail
            DurationMinutes: 30,
            AppointmentTypeId: new AppointmentTypeId(1),
            AppointmentTypeName: "TestType",    // required by your DTO
            IsUrgent: false,
            Notes: null,
            Status: AppointmentStatus.Scheduled,  // adjust enum member if yours differs
            RowVersion: null);

        var resp = await _http.PostAsJsonAsync("/api/scheduling/appointments", req);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
