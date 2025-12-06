using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Core.Entities;
using GIPractice.Core.Enums;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestsController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/tests?patientId=123
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestDto>>> Get([FromQuery] int? patientId)
    {
        var query = _db.Tests
            .AsNoTracking()
            .Include(t => t.Patient)
            .Include(t => t.Lab)
            .Include(t => t.Doctor)
            .Include(t => t.Findings)
            .AsQueryable();

        if (patientId.HasValue)
            query = query.Where(t => t.PatientId == patientId.Value);

        var tests = await query
            .OrderByDescending(t => t.PerformedAtUtc)
            .ToListAsync();

        var dto = _mapper.Map<List<TestDto>>(tests);
        return Ok(dto);
    }

    // GET /api/tests/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TestDto>> Get(int id)
    {
        var t = await _db.Tests
            .Include(x => x.Patient)
            .Include(x => x.Lab)
            .Include(x => x.Doctor)
            .Include(x => x.Findings)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (t == null)
            return NotFound();

        var dto = _mapper.Map<TestDto>(t);
        return Ok(dto);
    }

    // POST /api/tests
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] TestCreateDto dto)
    {
        // Patient required
        var patientExists = await _db.Patients
            .AnyAsync(p => p.Id == dto.PatientId);
        if (!patientExists)
        {
            ModelState.AddModelError(nameof(dto.PatientId), "Patient does not exist.");
            return ValidationProblem(ModelState);
        }

        // Optional lab
        if (dto.LabId.HasValue)
        {
            var labExists = await _db.Labs
                .AnyAsync(l => l.Id == dto.LabId.Value);
            if (!labExists)
            {
                ModelState.AddModelError(nameof(dto.LabId), "Lab does not exist.");
                return ValidationProblem(ModelState);
            }
        }

        // Optional doctor
        if (dto.DoctorId.HasValue)
        {
            var doctorExists = await _db.Doctors
                .AnyAsync(d => d.Id == dto.DoctorId.Value);
            if (!doctorExists)
            {
                ModelState.AddModelError(nameof(dto.DoctorId), "Doctor does not exist.");
                return ValidationProblem(ModelState);
            }
        }

        var test = new Test
        {
            PatientId = dto.PatientId,
            TestType = dto.TestType,
            PerformedAtUtc = dto.PerformedAtUtc,
            QualitativeResult = dto.QualitativeResult,
            QuantitativeResult = dto.QuantitativeResult,
            LabId = dto.LabId,
            DoctorId = dto.DoctorId,
            FileId = dto.FileId,
            Notes = dto.Notes
        };

        // Attach findings
        if (dto.FindingIds?.Count > 0)
        {
            var findings = await _db.Findings
                .Where(f => dto.FindingIds.Contains(f.Id))
                .ToListAsync();

            var missing = dto.FindingIds
                .Except(findings.Select(f => f.Id))
                .ToList();

            if (missing.Count > 0)
            {
                ModelState.AddModelError(nameof(dto.FindingIds),
                    "One or more finding IDs do not exist.");
                return ValidationProblem(ModelState);
            }

            test.Findings = findings;
        }

        _db.Tests.Add(test);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = test.Id }, null);
    }

    // PUT /api/tests/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] TestCreateDto dto)
    {
        var t = await _db.Tests
            .Include(x => x.Findings)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (t == null)
            return NotFound();

        // Patient check
        var patientExists = await _db.Patients
            .AnyAsync(p => p.Id == dto.PatientId);
        if (!patientExists)
        {
            ModelState.AddModelError(nameof(dto.PatientId), "Patient does not exist.");
            return ValidationProblem(ModelState);
        }

        // Lab optional
        if (dto.LabId.HasValue)
        {
            var labExists = await _db.Labs
                .AnyAsync(l => l.Id == dto.LabId.Value);
            if (!labExists)
            {
                ModelState.AddModelError(nameof(dto.LabId), "Lab does not exist.");
                return ValidationProblem(ModelState);
            }
        }

        // Doctor optional
        if (dto.DoctorId.HasValue)
        {
            var doctorExists = await _db.Doctors
                .AnyAsync(d => d.Id == dto.DoctorId.Value);
            if (!doctorExists)
            {
                ModelState.AddModelError(nameof(dto.DoctorId), "Doctor does not exist.");
                return ValidationProblem(ModelState);
            }
        }

        t.PatientId = dto.PatientId;
        t.TestType = dto.TestType;
        t.PerformedAtUtc = dto.PerformedAtUtc;
        t.QualitativeResult = dto.QualitativeResult;
        t.QuantitativeResult = dto.QuantitativeResult;
        t.LabId = dto.LabId;
        t.DoctorId = dto.DoctorId;
        t.FileId = dto.FileId;
        t.Notes = dto.Notes;

        // Replace findings set
        t.Findings.Clear();
        if (dto.FindingIds?.Count > 0)
        {
            var findings = await _db.Findings
                .Where(f => dto.FindingIds.Contains(f.Id))
                .ToListAsync();

            var missing = dto.FindingIds
                .Except(findings.Select(f => f.Id))
                .ToList();

            if (missing.Count > 0)
            {
                ModelState.AddModelError(nameof(dto.FindingIds),
                    "One or more finding IDs do not exist.");
                return ValidationProblem(ModelState);
            }

            foreach (var f in findings)
                t.Findings.Add(f);
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/tests/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var t = await _db.Tests.FindAsync(id);
        if (t == null)
            return NotFound();

        _db.Tests.Remove(t);
        await _db.SaveChangesAsync();

        return NoContent();
    }
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<TestListItemDto>>> GetList(
    [FromQuery] int? patientId = null,
    [FromQuery] DateTime? fromUtc = null,
    [FromQuery] DateTime? toUtc = null,
    [FromQuery] TestType? type = null,
    [FromQuery] QualitativeTestResults? qualitative = null)
    {
        var query = _db.Tests
            .AsNoTracking()
            .Include(t => t.Patient)
            .AsQueryable();

        if (patientId.HasValue)
            query = query.Where(t => t.PatientId == patientId.Value);

        if (fromUtc.HasValue)
            query = query.Where(t => t.PerformedAtUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(t => t.PerformedAtUtc <= toUtc.Value);

        if (type.HasValue)
            query = query.Where(t => t.TestType == type.Value);

        if (qualitative.HasValue)
            query = query.Where(t => t.QualitativeResult == qualitative.Value);

        var tests = await query
            .OrderByDescending(t => t.PerformedAtUtc)
            .ThenBy(t => t.Id)
            .ToListAsync();

        var dto = _mapper.Map<List<TestListItemDto>>(tests);
        return Ok(dto);
    }

}
