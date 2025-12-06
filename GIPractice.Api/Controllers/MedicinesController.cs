using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Core.Entities;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicinesController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/medicines?search=
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicineDto>>> GetMedicines(
        [FromQuery] string? search = null)
    {
        var query = _db.Medicines
            .Include(m => m.ActiveSubstances)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(m =>
                m.BrandName.Contains(search) ||
                m.ActiveSubstances.Any(a => a.Name.Contains(search)));
        }

        var list = await query
            .OrderBy(m => m.BrandName)
            .ToListAsync();

        var dto = _mapper.Map<List<MedicineDto>>(list);
        return Ok(dto);
    }

    // GET /api/medicines/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MedicineDto>> GetMedicine(int id)
    {
        var med = await _db.Medicines
            .Include(m => m.ActiveSubstances)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (med == null)
            return NotFound();

        var dto = _mapper.Map<MedicineDto>(med);
        return Ok(dto);
    }

    // POST /api/medicines
    [HttpPost]
    public async Task<ActionResult> CreateMedicine([FromBody] MedicineCreateUpdateDto dto)
    {
        var med = new Medicine
        {
            BrandName = dto.BrandName.Trim(),
            Type = dto.Type
        };

        if (dto.ActiveSubstanceIds?.Count > 0)
        {
            var subs = await _db.ActiveSubstances
                .Where(a => dto.ActiveSubstanceIds.Contains(a.Id))
                .ToListAsync();

            med.ActiveSubstances = subs;
        }

        _db.Medicines.Add(med);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMedicine), new { id = med.Id }, null);
    }

    // PUT /api/medicines/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateMedicine(int id, [FromBody] MedicineCreateUpdateDto dto)
    {
        var med = await _db.Medicines
            .Include(m => m.ActiveSubstances)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (med == null)
            return NotFound();

        med.BrandName = dto.BrandName.Trim();
        med.Type = dto.Type;

        // Refresh many-to-many ActiveSubstances
        med.ActiveSubstances.Clear();

        if (dto.ActiveSubstanceIds?.Count > 0)
        {
            var subs = await _db.ActiveSubstances
                .Where(a => dto.ActiveSubstanceIds.Contains(a.Id))
                .ToListAsync();

            foreach (var s in subs)
                med.ActiveSubstances.Add(s);
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/medicines/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteMedicine(int id)
    {
        var med = await _db.Medicines.FindAsync(id);
        if (med == null)
            return NotFound();

        _db.Medicines.Remove(med);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
