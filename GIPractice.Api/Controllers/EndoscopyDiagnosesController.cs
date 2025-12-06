using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/endoscopies/{endoscopyId:int}/diagnoses")]
public class EndoscopyDiagnosesController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/endoscopies/{endoscopyId}/diagnoses
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiagnosisDto>>> GetForEndoscopy(int endoscopyId)
    {
        var endoscopy = await _db.Endoscopies
            .Include(e => e.Diagnoses)
            .FirstOrDefaultAsync(e => e.Id == endoscopyId);

        if (endoscopy == null)
            return NotFound("Endoscopy not found.");

        var dto = _mapper.Map<List<DiagnosisDto>>(endoscopy.Diagnoses);
        return Ok(dto);
    }

    // POST /api/endoscopies/{endoscopyId}/diagnoses
    // body: { "diagnosisId": 123 }
    [HttpPost]
    public async Task<ActionResult> AddForEndoscopy(
        int endoscopyId,
        [FromBody] PatientDiagnosisAssignDto dto)
    {
        var endoscopy = await _db.Endoscopies
            .Include(e => e.Diagnoses)
            .FirstOrDefaultAsync(e => e.Id == endoscopyId);

        if (endoscopy == null)
            return NotFound("Endoscopy not found.");

        var diagnosis = await _db.Diagnoses
            .FirstOrDefaultAsync(d => d.Id == dto.DiagnosisId);

        if (diagnosis == null)
        {
            ModelState.AddModelError(nameof(dto.DiagnosisId), "Diagnosis does not exist.");
            return ValidationProblem(ModelState);
        }

        if (!endoscopy.Diagnoses.Any(d => d.Id == diagnosis.Id))
        {
            endoscopy.Diagnoses.Add(diagnosis);
            await _db.SaveChangesAsync();
        }

        return NoContent();
    }

    // DELETE /api/endoscopies/{endoscopyId}/diagnoses/{diagnosisId}
    [HttpDelete("{diagnosisId:int}")]
    public async Task<ActionResult> RemoveForEndoscopy(int endoscopyId, int diagnosisId)
    {
        var endoscopy = await _db.Endoscopies
            .Include(e => e.Diagnoses)
            .FirstOrDefaultAsync(e => e.Id == endoscopyId);

        if (endoscopy == null)
            return NotFound("Endoscopy not found.");

        var diagnosis = endoscopy.Diagnoses.FirstOrDefault(d => d.Id == diagnosisId);
        if (diagnosis == null)
            return NotFound("Diagnosis not linked to this endoscopy.");

        endoscopy.Diagnoses.Remove(diagnosis);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
