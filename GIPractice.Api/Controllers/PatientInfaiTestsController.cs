using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/patients/{patientId:int}/infai-tests")]
public class PatientInfaiTestsController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/patients/{patientId}/infai-tests
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InfaiTestDto>>> GetForPatient(int patientId)
    {
        var exists = await _db.Patients.AnyAsync(p => p.Id == patientId);
        if (!exists)
            return NotFound("Patient not found.");

        var tests = await _db.InfaiTests
            .Where(t => t.PatientId == patientId)
            .Include(t => t.Patient)
            .Include(t => t.File)
            .OrderByDescending(t => t.PerformedAtUtc)
            .ToListAsync();

        var dto = _mapper.Map<List<InfaiTestDto>>(tests);
        return Ok(dto);
    }
}
