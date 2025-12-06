using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/patients/{patientId:int}/endoscopies")]
public class PatientEndoscopiesController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/patients/{patientId}/endoscopies
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EndoscopyDto>>> GetForPatient(int patientId)
    {
        // Ensure patient exists
        var exists = await _db.Patients
            .AsNoTracking()
            .AnyAsync(p => p.Id == patientId);

        if (!exists)
            return NotFound("Patient not found.");

        var endoscopies = await _db.Endoscopies
            .Where(e => e.PatientId == patientId)
            .Include(e => e.Observations).ThenInclude(o => o.Finding)
            .Include(e => e.Observations).ThenInclude(o => o.OrganArea)
            .Include(e => e.BiopsyBottles).ThenInclude(bb => bb.OrganAreas)
            .Include(e => e.Report)
            .Include(e => e.MediaFiles)
            .AsNoTracking()
            .OrderByDescending(e => e.PerformedAtUtc)
            .ThenBy(e => e.Id)
            .ToListAsync();

        var dto = _mapper.Map<List<EndoscopyDto>>(endoscopies);
        return Ok(dto);
    }
}
