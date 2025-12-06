using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Core.Entities;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PreparationProtocolsController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/preparationprotocols?includeInactive=false
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PreparationProtocolDto>>> GetAll(
        [FromQuery] bool includeInactive = false)
    {
        var query = _db.PreparationProtocols.AsNoTracking();

        if (!includeInactive)
            query = query.Where(p => p.IsActive);

        var list = await query
            .OrderBy(p => p.EndoscopyType)
            .ThenBy(p => p.Name)
            .ToListAsync();

        var dto = _mapper.Map<List<PreparationProtocolDto>>(list);
        return Ok(dto);
    }

    // GET /api/preparationprotocols/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PreparationProtocolDto>> GetById(int id)
    {
        var entity = await _db.PreparationProtocols
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (entity == null)
            return NotFound();

        var dto = _mapper.Map<PreparationProtocolDto>(entity);
        return Ok(dto);
    }

    // POST /api/preparationprotocols
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] PreparationProtocolCreateUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            ModelState.AddModelError(nameof(dto.Name), "Name is required.");
            return ValidationProblem(ModelState);
        }

        if (string.IsNullOrWhiteSpace(dto.Instructions))
        {
            ModelState.AddModelError(nameof(dto.Instructions), "Instructions are required.");
            return ValidationProblem(ModelState);
        }

        var entity = new PreparationProtocol
        {
            Name = dto.Name.Trim(),
            EndoscopyType = dto.EndoscopyType,
            Instructions = dto.Instructions,
            IncludesBowelCleaning = dto.IncludesBowelCleaning,
            MinimumFastingHours = dto.MinimumFastingHours,
            IsActive = dto.IsActive
        };

        _db.PreparationProtocols.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, null);
    }

    // PUT /api/preparationprotocols/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] PreparationProtocolCreateUpdateDto dto)
    {
        var entity = await _db.PreparationProtocols.FirstOrDefaultAsync(p => p.Id == id);
        if (entity == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            ModelState.AddModelError(nameof(dto.Name), "Name is required.");
            return ValidationProblem(ModelState);
        }

        if (string.IsNullOrWhiteSpace(dto.Instructions))
        {
            ModelState.AddModelError(nameof(dto.Instructions), "Instructions are required.");
            return ValidationProblem(ModelState);
        }

        entity.Name = dto.Name.Trim();
        entity.EndoscopyType = dto.EndoscopyType;
        entity.Instructions = dto.Instructions;
        entity.IncludesBowelCleaning = dto.IncludesBowelCleaning;
        entity.MinimumFastingHours = dto.MinimumFastingHours;
        entity.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/preparationprotocols/{id}
    //
    // Soft delete: just mark IsActive = false so existing appointments
    // referencing the protocol don't break, but it stops appearing in lookups.
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var entity = await _db.PreparationProtocols.FirstOrDefaultAsync(p => p.Id == id);
        if (entity == null)
            return NotFound();

        if (!entity.IsActive)
        {
            // Already inactive; nothing to do.
            return NoContent();
        }

        entity.IsActive = false;
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
