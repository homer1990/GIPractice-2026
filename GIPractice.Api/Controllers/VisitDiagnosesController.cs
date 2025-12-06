using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/visits/{visitId:int}/diagnoses")]
public class VisitDiagnosesController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/visits/{visitId}/diagnoses
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiagnosisDto>>> GetForVisit(int visitId)
    {
        var visit = await _db.Visits
            .Include(v => v.Diagnoses)
            .FirstOrDefaultAsync(v => v.Id == visitId);

        if (visit == null)
            return NotFound("Visit not found.");

        var dto = _mapper.Map<List<DiagnosisDto>>(visit.Diagnoses);
        return Ok(dto);
    }

    // POST /api/visits/{visitId}/diagnoses
    // body: { "diagnosisId": 123 }
    [HttpPost]
    public async Task<ActionResult> AddForVisit(
        int visitId,
        [FromBody] PatientDiagnosisAssignDto dto)
    {
        var visit = await _db.Visits
            .Include(v => v.Diagnoses)
            .FirstOrDefaultAsync(v => v.Id == visitId);

        if (visit == null)
            return NotFound("Visit not found.");

        var diagnosis = await _db.Diagnoses
            .FirstOrDefaultAsync(d => d.Id == dto.DiagnosisId);

        if (diagnosis == null)
        {
            ModelState.AddModelError(nameof(dto.DiagnosisId), "Diagnosis does not exist.");
            return ValidationProblem(ModelState);
        }

        // Idempotent add
        if (!visit.Diagnoses.Any(d => d.Id == diagnosis.Id))
        {
            visit.Diagnoses.Add(diagnosis);
            await _db.SaveChangesAsync();
        }

        return NoContent();
    }

    // DELETE /api/visits/{visitId}/diagnoses/{diagnosisId}
    [HttpDelete("{diagnosisId:int}")]
    public async Task<ActionResult> RemoveForVisit(int visitId, int diagnosisId)
    {
        var visit = await _db.Visits
            .Include(v => v.Diagnoses)
            .FirstOrDefaultAsync(v => v.Id == visitId);

        if (visit == null)
            return NotFound("Visit not found.");

        var diagnosis = visit.Diagnoses.FirstOrDefault(d => d.Id == diagnosisId);
        if (diagnosis == null)
            return NotFound("Diagnosis not linked to this visit.");

        visit.Diagnoses.Remove(diagnosis);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
