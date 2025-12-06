using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/patients/{patientId:int}/operations")]
public class PatientOperationsController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/patients/{patientId}/operations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OperationDto>>> GetForPatient(int patientId)
    {
        var exists = await _db.Patients.AnyAsync(p => p.Id == patientId);
        if (!exists)
            return NotFound("Patient not found.");

        var ops = await _db.Operations
            .Where(o => o.PatientId == patientId)
            .Include(o => o.Patient)
            .Include(o => o.Doctor)
            .Include(o => o.File)
            .OrderByDescending(o => o.DateAndTimeUtc)
            .ToListAsync();

        var dto = _mapper.Map<List<OperationDto>>(ops);
        return Ok(dto);
    }
}
