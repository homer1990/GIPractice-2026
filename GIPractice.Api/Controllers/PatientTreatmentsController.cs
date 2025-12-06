using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/patients/{patientId:int}/treatments")]
public class PatientTreatmentsController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/patients/{patientId}/treatments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TreatmentDto>>> GetForPatient(int patientId)
    {
        var patient = await _db.Patients
            .Include(p => p.Treatments)
                .ThenInclude(t => t.Doctor)
            .Include(p => p.Treatments)
                .ThenInclude(t => t.Diagnoses)
            .Include(p => p.Treatments)
                .ThenInclude(t => t.Medications)
            .FirstOrDefaultAsync(p => p.Id == patientId);

        if (patient == null)
            return NotFound("Patient not found.");

        var dto = _mapper.Map<List<TreatmentDto>>(patient.Treatments);
        return Ok(dto);
    }
}
