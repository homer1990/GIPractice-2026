using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Core.Entities;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InfaiTestsController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/infaiTests?patientId=123
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InfaiTestDto>>> Get([FromQuery] int? patientId)
    {
        var query = _db.InfaiTests
            .AsNoTracking()
            .Include(i => i.Patient)
            .Include(i => i.File)
            .AsQueryable();

        if (patientId.HasValue)
            query = query.Where(i => i.PatientId == patientId.Value);

        var tests = await query
            .OrderByDescending(i => i.PerformedAtUtc)
            .ToListAsync();

        var dto = _mapper.Map<List<InfaiTestDto>>(tests);
        return Ok(dto);
    }

    // GET /api/infaiTests/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<InfaiTestDto>> Get(int id)
    {
        var t = await _db.InfaiTests
            .Include(i => i.Patient)
            .Include(i => i.File)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (t == null)
            return NotFound();

        var dto = _mapper.Map<InfaiTestDto>(t);
        return Ok(dto);
    }

    // POST /api/infaiTests
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] InfaiTestCreateDto dto)
    {
        var patientExists = await _db.Patients
            .AnyAsync(p => p.Id == dto.PatientId);
        if (!patientExists)
        {
            ModelState.AddModelError(nameof(dto.PatientId), "Patient does not exist.");
            return ValidationProblem(ModelState);
        }

        if (dto.FileId.HasValue)
        {
            var fileExists = await _db.MediaFiles
                .AnyAsync(f => f.Id == dto.FileId.Value);
            if (!fileExists)
            {
                ModelState.AddModelError(nameof(dto.FileId), "File does not exist.");
                return ValidationProblem(ModelState);
            }
        }

        var test = new InfaiTest
        {
            PatientId = dto.PatientId,
            PerformedAtUtc = dto.PerformedAtUtc,
            Result = dto.Result,
            FileId = dto.FileId,
            Notes = dto.Notes
        };

        _db.InfaiTests.Add(test);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = test.Id }, null);
    }

    // PUT /api/infaiTests/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] InfaiTestCreateDto dto)
    {
        var t = await _db.InfaiTests.FirstOrDefaultAsync(i => i.Id == id);
        if (t == null)
            return NotFound();

        var patientExists = await _db.Patients
            .AnyAsync(p => p.Id == dto.PatientId);
        if (!patientExists)
        {
            ModelState.AddModelError(nameof(dto.PatientId), "Patient does not exist.");
            return ValidationProblem(ModelState);
        }

        if (dto.FileId.HasValue)
        {
            var fileExists = await _db.MediaFiles
                .AnyAsync(f => f.Id == dto.FileId.Value);
            if (!fileExists)
            {
                ModelState.AddModelError(nameof(dto.FileId), "File does not exist.");
                return ValidationProblem(ModelState);
            }
        }

        t.PatientId = dto.PatientId;
        t.PerformedAtUtc = dto.PerformedAtUtc;
        t.Result = dto.Result;
        t.FileId = dto.FileId;
        t.Notes = dto.Notes;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/infaiTests/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var t = await _db.InfaiTests.FindAsync(id);
        if (t == null)
            return NotFound();

        _db.InfaiTests.Remove(t);
        await _db.SaveChangesAsync();

        return NoContent();
    }
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<InfaiTestListItemDto>>> GetList(
    [FromQuery] int? patientId = null,
    [FromQuery] DateTime? fromUtc = null,
    [FromQuery] DateTime? toUtc = null)
    {
        var query = _db.InfaiTests
            .AsNoTracking()
            .Include(i => i.Patient)
            .AsQueryable();

        if (patientId.HasValue)
            query = query.Where(i => i.PatientId == patientId.Value);

        if (fromUtc.HasValue)
            query = query.Where(i => i.PerformedAtUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(i => i.PerformedAtUtc <= toUtc.Value);

        var infai = await query
            .OrderByDescending(i => i.PerformedAtUtc)
            .ThenBy(i => i.Id)
            .ToListAsync();

        var dto = _mapper.Map<List<InfaiTestListItemDto>>(infai);
        return Ok(dto);
    }
}
