using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Core.Entities;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiagnosesController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/diagnoses
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiagnosisDto>>> Get()
    {
        var diagnoses = await _db.Diagnoses
            .AsNoTracking()
            .OrderBy(d => d.Code)
            .ThenBy(d => d.Name)
            .ToListAsync();

        var dto = _mapper.Map<List<DiagnosisDto>>(diagnoses);
        return Ok(dto);
    }

    // GET /api/diagnoses/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DiagnosisDto>> Get(int id)
    {
        var d = await _db.Diagnoses.FindAsync(id);
        if (d == null)
            return NotFound();

        return Ok(_mapper.Map<DiagnosisDto>(d));
    }

    // POST /api/diagnoses
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] DiagnosisCreateDto dto)
    {
        // Optional: uniqueness check on Code+Standard
        var exists = await _db.Diagnoses
            .AnyAsync(d => d.Code == dto.Code && d.Standard == dto.Standard);

        if (exists)
        {
            ModelState.AddModelError(nameof(dto.Code),
                "A diagnosis with the same code and standard already exists.");
            return ValidationProblem(ModelState);
        }

        var entity = _mapper.Map<Diagnosis>(dto);
        _db.Diagnoses.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = entity.Id }, null);
    }

    // PUT /api/diagnoses/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] DiagnosisCreateDto dto)
    {
        var d = await _db.Diagnoses.FindAsync(id);
        if (d == null)
            return NotFound();

        d.Name = dto.Name;
        d.Code = dto.Code;
        d.Standard = dto.Standard;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/diagnoses/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var d = await _db.Diagnoses.FindAsync(id);
        if (d == null)
            return NotFound();

        _db.Diagnoses.Remove(d);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
