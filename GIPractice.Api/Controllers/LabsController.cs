using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Core.Entities;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LabsController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/labs?search=
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LabDto>>> GetLabs([FromQuery] string? search = null)
    {
        var query = _db.Labs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(l => l.Name.Contains(search));
        }

        var labs = await query
            .OrderBy(l => l.Name)
            .ToListAsync();

        var dto = _mapper.Map<List<LabDto>>(labs);
        return Ok(dto);
    }

    // GET /api/labs/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LabDto>> GetLab(int id)
    {
        var lab = await _db.Labs
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lab == null)
            return NotFound();

        var dto = _mapper.Map<LabDto>(lab);
        return Ok(dto);
    }

    // POST /api/labs
    [HttpPost]
    public async Task<ActionResult> CreateLab([FromBody] LabCreateUpdateDto dto)
    {
        var lab = new Lab
        {
            Name = dto.Name.Trim(),
            LabTypes = dto.LabTypes,
            Address = dto.Address,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email
        };

        _db.Labs.Add(lab);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLab), new { id = lab.Id }, null);
    }

    // PUT /api/labs/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateLab(int id, [FromBody] LabCreateUpdateDto dto)
    {
        var lab = await _db.Labs.FindAsync(id);
        if (lab == null)
            return NotFound();

        lab.Name = dto.Name.Trim();
        lab.LabTypes = dto.LabTypes;
        lab.Address = dto.Address;
        lab.PhoneNumber = dto.PhoneNumber;
        lab.Email = dto.Email;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/labs/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteLab(int id)
    {
        var lab = await _db.Labs.FindAsync(id);
        if (lab == null)
            return NotFound();

        _db.Labs.Remove(lab);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
