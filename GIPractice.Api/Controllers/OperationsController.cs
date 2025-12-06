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
public class OperationsController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/operations?patientId=123
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OperationDto>>> Get([FromQuery] int? patientId)
    {
        var query = _db.Operations
            .AsNoTracking()
            .Include(o => o.Patient)
            .Include(o => o.Doctor)
            .Include(o => o.File)
            .AsQueryable();

        if (patientId.HasValue)
            query = query.Where(o => o.PatientId == patientId.Value);

        var ops = await query
            .OrderByDescending(o => o.DateAndTimeUtc)
            .ToListAsync();

        var dto = _mapper.Map<List<OperationDto>>(ops);
        return Ok(dto);
    }

    // GET /api/operations/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OperationDto>> Get(int id)
    {
        var o = await _db.Operations
            .Include(x => x.Patient)
            .Include(x => x.Doctor)
            .Include(x => x.File)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (o == null)
            return NotFound();

        var dto = _mapper.Map<OperationDto>(o);
        return Ok(dto);
    }

    // POST /api/operations
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] OperationCreateDto dto)
    {
        var patientExists = await _db.Patients
            .AnyAsync(p => p.Id == dto.PatientId);
        if (!patientExists)
        {
            ModelState.AddModelError(nameof(dto.PatientId), "Patient does not exist.");
            return ValidationProblem(ModelState);
        }

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

        var op = new Operation
        {
            PatientId = dto.PatientId,
            DateAndTimeUtc = dto.DateAndTimeUtc,
            Type = dto.Type,
            Outcome = dto.Outcome,
            DoctorId = dto.DoctorId,
            FileId = dto.FileId
        };

        _db.Operations.Add(op);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = op.Id }, null);
    }

    // PUT /api/operations/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] OperationCreateDto dto)
    {
        var o = await _db.Operations.FirstOrDefaultAsync(x => x.Id == id);
        if (o == null)
            return NotFound();

        var patientExists = await _db.Patients
            .AnyAsync(p => p.Id == dto.PatientId);
        if (!patientExists)
        {
            ModelState.AddModelError(nameof(dto.PatientId), "Patient does not exist.");
            return ValidationProblem(ModelState);
        }

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

        o.PatientId = dto.PatientId;
        o.DateAndTimeUtc = dto.DateAndTimeUtc;
        o.Type = dto.Type;
        o.Outcome = dto.Outcome;
        o.DoctorId = dto.DoctorId;
        o.FileId = dto.FileId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/operations/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var o = await _db.Operations.FindAsync(id);
        if (o == null)
            return NotFound();

        _db.Operations.Remove(o);
        await _db.SaveChangesAsync();

        return NoContent();
    }
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<OperationListItemDto>>> GetList(
    [FromQuery] int? patientId = null,
    [FromQuery] DateTime? fromUtc = null,
    [FromQuery] DateTime? toUtc = null,
    [FromQuery] OperationTypes? type = null,
    [FromQuery] Outcomes? outcome = null)
    {
        var query = _db.Operations
            .AsNoTracking()
            .Include(o => o.Patient)
            .AsQueryable();

        if (patientId.HasValue)
            query = query.Where(o => o.PatientId == patientId.Value);

        if (fromUtc.HasValue)
            query = query.Where(o => o.DateAndTimeUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(o => o.DateAndTimeUtc <= toUtc.Value);

        if (type.HasValue)
            query = query.Where(o => o.Type == type.Value);

        if (outcome.HasValue)
            query = query.Where(o => o.Outcome == outcome.Value);

        var operations = await query
            .OrderByDescending(o => o.DateAndTimeUtc)
            .ThenBy(o => o.Id)
            .ToListAsync();

        var dto = _mapper.Map<List<OperationListItemDto>>(operations);
        return Ok(dto);
    }
}
