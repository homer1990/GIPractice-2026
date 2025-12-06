using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Core.Entities;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActiveSubstancesController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/activesubstances?search=
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActiveSubstanceDto>>> GetActiveSubstances(
        [FromQuery] string? search = null)
    {
        var query = _db.ActiveSubstances.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(a => a.Name.Contains(search));
        }

        var list = await query
            .OrderBy(a => a.Name)
            .ToListAsync();

        var dto = _mapper.Map<List<ActiveSubstanceDto>>(list);
        return Ok(dto);
    }

    // GET /api/activesubstances/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ActiveSubstanceDto>> GetActiveSubstance(int id)
    {
        var entity = await _db.ActiveSubstances
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (entity == null)
            return NotFound();

        var dto = _mapper.Map<ActiveSubstanceDto>(entity);
        return Ok(dto);
    }

    // POST /api/activesubstances
    [HttpPost]
    public async Task<ActionResult> CreateActiveSubstance([FromBody] ActiveSubstanceCreateUpdateDto dto)
    {
        var entity = new ActiveSubstance
        {
            Name = dto.Name.Trim(),
            Description = dto.Description
        };

        _db.ActiveSubstances.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetActiveSubstance), new { id = entity.Id }, null);
    }

    // PUT /api/activesubstances/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateActiveSubstance(int id, [FromBody] ActiveSubstanceCreateUpdateDto dto)
    {
        var entity = await _db.ActiveSubstances.FindAsync(id);
        if (entity == null)
            return NotFound();

        entity.Name = dto.Name.Trim();
        entity.Description = dto.Description;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/activesubstances/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteActiveSubstance(int id)
    {
        var entity = await _db.ActiveSubstances.FindAsync(id);
        if (entity == null)
            return NotFound();

        _db.ActiveSubstances.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
