using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Core.Entities;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TreatmentsController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/treatments?patientId=123 (optional filter)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TreatmentDto>>> Get([FromQuery] int? patientId)
    {
        var query = _db.Treatments
            .AsNoTracking()
            .Include(t => t.Patient)
            .Include(t => t.Doctor)
            .Include(t => t.Diagnoses)
            .Include(t => t.Medications)
            .AsQueryable();

        if (patientId.HasValue)
            query = query.Where(t => t.PatientId == patientId.Value);

        var treatments = await query
            .OrderByDescending(t => t.StartUtc)
            .ToListAsync();

        var dto = _mapper.Map<List<TreatmentDto>>(treatments);
        return Ok(dto);
    }

    // GET /api/treatments/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TreatmentDto>> Get(int id)
    {
        var t = await _db.Treatments
            .Include(t => t.Patient)
            .Include(t => t.Doctor)
            .Include(t => t.Diagnoses)
            .Include(t => t.Medications)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (t == null)
            return NotFound();

        var dto = _mapper.Map<TreatmentDto>(t);
        return Ok(dto);
    }

    // POST /api/treatments
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] TreatmentCreateDto dto)
    {
        // Validate patient
        var patientExists = await _db.Patients
            .AnyAsync(p => p.Id == dto.PatientId);
        if (!patientExists)
        {
            ModelState.AddModelError(nameof(dto.PatientId), "Patient does not exist.");
            return ValidationProblem(ModelState);
        }

        // Validate doctor
        var doctorExists = await _db.Doctors
            .AnyAsync(d => d.Id == dto.DoctorId);
        if (!doctorExists)
        {
            ModelState.AddModelError(nameof(dto.DoctorId), "Doctor does not exist.");
            return ValidationProblem(ModelState);
        }

        var treatment = new Treatment
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            StartUtc = dto.StartUtc,
            EndUtc = dto.EndUtc,
            Outcome = dto.Outcome,
            IsChronic = dto.IsChronic,
            Notes = dto.Notes
        };

        // Attach diagnoses
        if (dto.DiagnosisIds?.Count > 0)
        {
            var diagnoses = await _db.Diagnoses
                .Where(d => dto.DiagnosisIds.Contains(d.Id))
                .ToListAsync();

            var missing = dto.DiagnosisIds
                .Except(diagnoses.Select(d => d.Id))
                .ToList();

            if (missing.Count > 0)
            {
                ModelState.AddModelError(nameof(dto.DiagnosisIds),
                    "One or more diagnosis IDs do not exist.");
                return ValidationProblem(ModelState);
            }

            treatment.Diagnoses = diagnoses;
        }

        // Attach medicines
        if (dto.MedicineIds?.Count > 0)
        {
            var meds = await _db.Medicines
                .Where(m => dto.MedicineIds.Contains(m.Id))
                .ToListAsync();

            var missing = dto.MedicineIds
                .Except(meds.Select(m => m.Id))
                .ToList();

            if (missing.Count > 0)
            {
                ModelState.AddModelError(nameof(dto.MedicineIds),
                    "One or more medicine IDs do not exist.");
                return ValidationProblem(ModelState);
            }

            treatment.Medications = meds;
        }

        _db.Treatments.Add(treatment);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = treatment.Id }, null);
    }

    // PUT /api/treatments/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] TreatmentCreateDto dto)
    {
        var t = await _db.Treatments
            .Include(x => x.Diagnoses)
            .Include(x => x.Medications)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (t == null)
            return NotFound();

        // Validate patient
        var patientExists = await _db.Patients
            .AnyAsync(p => p.Id == dto.PatientId);
        if (!patientExists)
        {
            ModelState.AddModelError(nameof(dto.PatientId), "Patient does not exist.");
            return ValidationProblem(ModelState);
        }

        // Validate doctor
        var doctorExists = await _db.Doctors
            .AnyAsync(d => d.Id == dto.DoctorId);
        if (!doctorExists)
        {
            ModelState.AddModelError(nameof(dto.DoctorId), "Doctor does not exist.");
            return ValidationProblem(ModelState);
        }

        t.PatientId = dto.PatientId;
        t.DoctorId = dto.DoctorId;
        t.StartUtc = dto.StartUtc;
        t.EndUtc = dto.EndUtc;
        t.Outcome = dto.Outcome;
        t.IsChronic = dto.IsChronic;
        t.Notes = dto.Notes;

        // Update diagnoses (replace set)
        t.Diagnoses.Clear();
        if (dto.DiagnosisIds?.Count > 0)
        {
            var diagnoses = await _db.Diagnoses
                .Where(d => dto.DiagnosisIds.Contains(d.Id))
                .ToListAsync();

            var missing = dto.DiagnosisIds
                .Except(diagnoses.Select(d => d.Id))
                .ToList();

            if (missing.Count > 0)
            {
                ModelState.AddModelError(nameof(dto.DiagnosisIds),
                    "One or more diagnosis IDs do not exist.");
                return ValidationProblem(ModelState);
            }

            foreach (var d in diagnoses)
                t.Diagnoses.Add(d);
        }

        // Update medicines (replace set)
        t.Medications.Clear();
        if (dto.MedicineIds?.Count > 0)
        {
            var meds = await _db.Medicines
                .Where(m => dto.MedicineIds.Contains(m.Id))
                .ToListAsync();

            var missing = dto.MedicineIds
                .Except(meds.Select(m => m.Id))
                .ToList();

            if (missing.Count > 0)
            {
                ModelState.AddModelError(nameof(dto.MedicineIds),
                    "One or more medicine IDs do not exist.");
                return ValidationProblem(ModelState);
            }

            foreach (var m in meds)
                t.Medications.Add(m);
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/treatments/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var t = await _db.Treatments.FindAsync(id);
        if (t == null)
            return NotFound();

        _db.Treatments.Remove(t);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
