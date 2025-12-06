using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/patients/{patientId:int}/diagnoses")]
public class PatientDiagnosesController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/patients/{patientId}/diagnoses
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiagnosisDto>>> GetForPatient(int patientId)
    {
        var patient = await _db.Patients
            .Include(p => p.Diagnoses)
            .FirstOrDefaultAsync(p => p.Id == patientId);

        if (patient == null)
            return NotFound("Patient not found.");

        var dto = _mapper.Map<List<DiagnosisDto>>(patient.Diagnoses);
        return Ok(dto);
    }

    // POST /api/patients/{patientId}/diagnoses
    // body: { "diagnosisId": 123 }
    [HttpPost]
    public async Task<ActionResult> AddForPatient(
        int patientId,
        [FromBody] PatientDiagnosisAssignDto dto)
    {
        var patient = await _db.Patients
            .Include(p => p.Diagnoses)
            .FirstOrDefaultAsync(p => p.Id == patientId);

        if (patient == null)
            return NotFound("Patient not found.");

        var diagnosis = await _db.Diagnoses
            .FirstOrDefaultAsync(d => d.Id == dto.DiagnosisId);

        if (diagnosis == null)
        {
            ModelState.AddModelError(nameof(dto.DiagnosisId), "Diagnosis does not exist.");
            return ValidationProblem(ModelState);
        }

        // Idempotent: if already linked, do nothing
        if (!patient.Diagnoses.Any(d => d.Id == diagnosis.Id))
        {
            patient.Diagnoses.Add(diagnosis);
            await _db.SaveChangesAsync();
        }

        return NoContent();
    }

    // DELETE /api/patients/{patientId}/diagnoses/{diagnosisId}
    [HttpDelete("{diagnosisId:int}")]
    public async Task<ActionResult> RemoveForPatient(int patientId, int diagnosisId)
    {
        var patient = await _db.Patients
            .Include(p => p.Diagnoses)
            .FirstOrDefaultAsync(p => p.Id == patientId);

        if (patient == null)
            return NotFound("Patient not found.");

        var diagnosis = patient.Diagnoses.FirstOrDefault(d => d.Id == diagnosisId);
        if (diagnosis == null)
            return NotFound("Diagnosis not linked to this patient.");

        patient.Diagnoses.Remove(diagnosis);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
