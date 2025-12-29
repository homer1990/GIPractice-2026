using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Scheduling;
using Xunit;

namespace GIPractice.Api.Tests;

public sealed class SchedulingAppointmentIdMismatchApiTests(TestApiFactory factory) : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _http = factory.CreateClient();

    [Fact]
    public async Task UpdateAppointment_WhenRouteIdDoesNotMatchBodyId_ShouldReturn400_ResultDto()
    {
        var dto = new AppointmentUpsertRequestDto(
            Id: new AppointmentId(999),
            PatientId: new PatientId(1),
            StartUtc: DateTime.UtcNow.AddDays(1),
            DurationMinutes: 30,
            AppointmentTypeId: new AppointmentTypeId(1),
            AppointmentTypeName: "TestType",
            IsUrgent: false,
            Notes: null,
            Status: AppointmentStatus.Scheduled,
            RowVersion: null);

        var resp = await _http.PutAsJsonAsync("/api/scheduling/appointments/1", dto);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await resp.Content.ReadFromJsonAsync<ResultDto<bool>>();
        body.Should().NotBeNull();
        body!.IsSuccess.Should().BeFalse();
        body.Error.Should().NotBeNull();
        body.Error!.Code.Should().Be("validation");
    }
}
